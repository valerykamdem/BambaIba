using System.Text;
using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Features.MediaBase.UploadMedia;
using BambaIba.Application.Settings;
using BambaIba.Domain.Entities.Audios;
using BambaIba.Domain.Entities.MediaBase;
using BambaIba.Domain.Entities.VideoQualities;
using BambaIba.Domain.Entities.Videos;
using BambaIba.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BambaIba.Worker;


public class MediaConsumer : BackgroundService
{
    private readonly ILogger<MediaConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMediaProcessingService _processingService;
    private readonly IMediaStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;

    public MediaConsumer(
        ILogger<MediaConsumer> logger,
        IServiceScopeFactory scopeFactory,
        IMediaProcessingService processingService,
        IMediaStorageService storageService,
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
            exchange: "media-exchange",
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken
        );

        // 3. Déclarer la queue
        await channel.QueueDeclareAsync(
            queue: "media-processing",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken
        );

        await channel.QueueBindAsync(
            queue: "media-processing",
            exchange: "media-exchange",
            routingKey: "media-processing",
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
                string[] parts = message.Split('|');
                var mediaId = Guid.Parse(parts[0]);
                string path = parts[1];

                _logger.LogInformation(" [x] Received video {MediaId}", mediaId);

                // Appel de ton traitement factorisé
                //await ProcessVideoAsync(videoId, stoppingToken);
                await ProcessMediaAsync(mediaId, path, stoppingToken);

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



    private async Task ProcessMediaAsync(Guid mediaId, string? thumbnailPath, CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediaRepository mediaRepository = scope.ServiceProvider.GetRequiredService<IMediaRepository>();
        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        IMediaStorageService storageService = scope.ServiceProvider.GetRequiredService<IMediaStorageService>();
        IMediaProcessingService processingService = scope.ServiceProvider.GetRequiredService<IMediaProcessingService>();
        IVideoQualityRepository videoQlRepository = scope.ServiceProvider.GetRequiredService<IVideoQualityRepository>();
        ILogger<UploadMediaCommandHandler> logger = scope.ServiceProvider.GetRequiredService<ILogger<UploadMediaCommandHandler>>();

        Media? media = await mediaRepository.GetMediaByIdAsync(mediaId, cancellationToken);
        if (media == null || string.IsNullOrEmpty(media.StoragePath))
        {
            logger.LogWarning("Media not found or has no storage path: {MediaId}", mediaId);
            return;
        }

        try
        {
            string localPath;

            if (media is Video video)
            {
                logger.LogInformation("Starting video processing for {VideoId}", mediaId);

                // Télécharger la vidéo
                localPath = await storageService.DownloadVideoAsync(video.StoragePath, cancellationToken);

                // Extraire la durée
                video.Duration = await processingService.GetDurationAsync(localPath);
                video.Status = MediaStatus.Ready;
                video.PublishedAt = DateTime.UtcNow;
                video.UpdatedAt = DateTime.UtcNow;

                // Générer miniature si absente
                if (string.IsNullOrEmpty(thumbnailPath))
                {
                    thumbnailPath = await processingService.GenerateThumbnailAsync(video.Id, localPath);
                    video.ThumbnailPath = thumbnailPath;
                }

                await mediaRepository.UpdateMediaStatus(video);

                // Transcoder en plusieurs qualités
                foreach (string quality in VideoQualitySetting.All)
                {
                    try
                    {
                        string qualityPath = await processingService.TranscodeVideoAsync(video.Id, video.StoragePath, quality, cancellationToken);
                        long qualitySize = await storageService.GetFileSizeAsync(qualityPath);

                        var videoQuality = new VideoQuality
                        {
                            Id = Guid.CreateVersion7(),
                            MediaId = video.Id,
                            Quality = quality,
                            StoragePath = qualityPath,
                            FileSize = qualitySize,
                            CreatedAt = DateTime.UtcNow
                        };

                        await videoQlRepository.AddVideoQuality(videoQuality);
                        await unitOfWork.SaveChangesAsync(cancellationToken);

                        logger.LogInformation("Transcoding completed for {Quality}: {QualityPath}", quality, qualityPath);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to transcode video {VideoId} to {Quality}", video.Id, quality);
                    }
                }
            }
            else if (media is Audio audio)
            {
                logger.LogInformation("Starting audio processing for {AudioId}", mediaId);

                // Télécharger l’audio
                localPath = await storageService.DownloadAudioAsync(audio.StoragePath, cancellationToken);

                try
                {
                    audio.Duration = await processingService.GetDurationAsync(localPath);
                    audio.Status = MediaStatus.Ready;
                    audio.PublishedAt = DateTime.UtcNow;
                    audio.UpdatedAt = DateTime.UtcNow;

                    await mediaRepository.UpdateMediaStatus(audio);

                    logger.LogInformation("Audio processing completed: {AudioId}", audio.Id);
                }
                finally
                {
                    if (File.Exists(localPath))
                        File.Delete(localPath);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process media {MediaId}", mediaId);

            if (media != null)
            {
                media.Status = MediaStatus.Failed;
                media.UpdatedAt = DateTime.UtcNow;
                await mediaRepository.UpdateMediaStatus(media);
            }
        }
    }


    private async Task ProcessVideoAsync(Guid videoId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting video processing for {VideoId}", videoId);

        try
        {
            // --- SCOPE 1: Charger la vidéo ---
            Media video;
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                IMediaRepository repo = scope.ServiceProvider.GetRequiredService<IMediaRepository>();
                video = await repo.GetMediaByIdAsync(videoId, cancellationToken);
            }

            if (video == null || string.IsNullOrEmpty(video.StoragePath))
            {
                _logger.LogWarning("Video not found or has no storage path: {VideoId}", videoId);
                return;
            }

            // 1. Durée
            video.Duration = await _processingService.GetDurationAsync(video.StoragePath);
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
            await UpdateMediaStatusAsync(videoId, MediaStatus.Ready, cancellationToken);
            _logger.LogInformation("Video processing completed successfully for {VideoId}", videoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process video {VideoId}", videoId);
            await UpdateMediaStatusAsync(videoId, MediaStatus.Failed, cancellationToken);
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

    private async Task UpdateMediaStatusAsync(Guid videoId, MediaStatus status, CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediaRepository repo = scope.ServiceProvider.GetRequiredService<IMediaRepository>();

        Media video = await repo.GetMediaByIdAsync(videoId, cancellationToken);
        if (video != null)
        {
            video.Status = status;
            video.UpdatedAt = DateTime.UtcNow;
            if (status == MediaStatus.Ready)
                video.PublishedAt = DateTime.UtcNow;

            await repo.UpdateMediaStatus(video);
        }
    }
}
