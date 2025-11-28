using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Settings;
using BambaIba.Domain.Enums;
using BambaIba.Domain.VideoQualities;
using BambaIba.Domain.Videos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BambaIba.Worker;



public class VideoConsumer : BackgroundService
{
    private readonly ILogger<VideoConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IVideoProcessingService _processingService;
    private readonly IVideoStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public VideoConsumer(
        ILogger<VideoConsumer> logger,
        IServiceScopeFactory scopeFactory,
        IVideoProcessingService processingService,
        IVideoStorageService storageService,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _processingService = processingService;
        _storageService = storageService;
        _unitOfWork = unitOfWork;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = Environment.GetEnvironmentVariable("RABBITMQ__HOST") ?? "localhost",
            UserName = Environment.GetEnvironmentVariable("RABBITMQ__USER") ?? "admin",
            Password = Environment.GetEnvironmentVariable("RABBITMQ__PASS") ?? "admin"
        };

        // 1. Connexion async
        await using IConnection connection = await factory.CreateConnectionAsync(stoppingToken);

        // 2. Channel async
        await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(
            exchange: "video-exchange",
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken
        );

        // 3. Déclarer la queue
        await channel.QueueDeclareAsync(
            queue: "video-processing",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken
        );

        await channel.QueueBindAsync(
            queue: "video-processing",
            exchange: "video-exchange",
            routingKey: "video-processing",
            cancellationToken: stoppingToken
        );

        // 4. Consumer
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                var videoId = Guid.Parse(message);

                _logger.LogInformation(" [x] Received video {VideoId}", videoId);

                // Appel de ton traitement factorisé
                await ProcessVideoAsync(videoId);

                // Ack async
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing message");
            }
        };

        // 5. Consommation async
        await channel.BasicConsumeAsync(
            queue: "video-processing",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        _logger.LogInformation(" [*] Worker started, waiting for messages...");
    }

    private async Task ProcessVideoAsync(Guid videoId)
    {
        _logger.LogInformation("Starting video processing for {VideoId}", videoId);

        try
        {
            // --- SCOPE 1: Charger la vidéo ---
            Video video;
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                IVideoRepository repo = scope.ServiceProvider.GetRequiredService<IVideoRepository>();
                video = await repo.GetVideoById(videoId);
            }

            if (video == null || string.IsNullOrEmpty(video.StoragePath))
            {
                _logger.LogWarning("Video not found or has no storage path: {VideoId}", videoId);
                return;
            }

            // 1. Durée
            video.Duration = await _processingService.GetVideoDurationAsync(video.StoragePath);
            _logger.LogInformation("Duration extracted: {Duration}", video.Duration);

            // 2. Miniature
            video.ThumbnailPath = await _processingService.GenerateThumbnailAsync(videoId, video.StoragePath);
            _logger.LogInformation("Thumbnail generated: {ThumbnailPath}", video.ThumbnailPath);

            await _unitOfWork.SaveChangesAsync();

            // 3. Transcodage
            foreach (string quality in VideoQualitySetting.All)
            {
                await ProcessQualityAsync(videoId, video.StoragePath, quality);
            }

            // 4. Mise à jour finale
            await UpdateVideoStatusAsync(videoId, MediaStatus.Ready);
            _logger.LogInformation("Video processing completed successfully for {VideoId}", videoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process video {VideoId}", videoId);
            await UpdateVideoStatusAsync(videoId, MediaStatus.Failed);
        }
    }

    private async Task ProcessQualityAsync(Guid videoId, string storagePath, string quality)
    {
        try
        {
            _logger.LogInformation("Transcoding {VideoId} to {Quality}", videoId, quality);

            string qualityPath = await _processingService.TranscodeVideoAsync(videoId, storagePath, quality);
            long qualitySize = await _storageService.GetFileSizeAsync(qualityPath);

            using IServiceScope scope = _scopeFactory.CreateScope();
            IVideoQualityRepository repo = scope.ServiceProvider.GetRequiredService<IVideoQualityRepository>();

            var videoQuality = new VideoQuality
            {
                Id = Guid.CreateVersion7(),
                MediaId = videoId,
                Quality = quality,
                StoragePath = qualityPath,
                FileSize = qualitySize,
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddVideoQuality(videoQuality);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Transcoding completed for {Quality}", quality);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to transcode {VideoId} to {Quality}", videoId, quality);
        }
    }

    private async Task UpdateVideoStatusAsync(Guid videoId, MediaStatus status)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IVideoRepository repo = scope.ServiceProvider.GetRequiredService<IVideoRepository>();

        Video video = await repo.GetVideoById(videoId);
        if (video != null)
        {
            video.Status = status;
            video.UpdatedAt = DateTime.UtcNow;
            if (status == MediaStatus.Ready)
                video.PublishedAt = DateTime.UtcNow;

            await repo.UpdateVideoStatus(video);
        }
    }
}
