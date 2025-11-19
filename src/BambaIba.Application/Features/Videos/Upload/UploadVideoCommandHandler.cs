using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Abstractions.Services;
using BambaIba.Application.Features.Audios.Upload;
using BambaIba.Application.Settings;
using BambaIba.Domain.Audios;
using BambaIba.Domain.Enums;
using BambaIba.Domain.VideoQualities;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace BambaIba.Application.Features.Videos.Upload;
public sealed class UploadVideoCommandHandler : ICommandHandler<UploadVideoCommand, Result<UploadVideoResult>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<UploadVideoCommandHandler> _logger;
    private readonly IUserContextService _userContextService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly VideoPublisher _videoPublisher;

    public UploadVideoCommandHandler(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<UploadVideoCommandHandler> logger,
        IUserContextService userContextService,
        IHttpContextAccessor httpContextAccessor,
        VideoPublisher videoPublisher
        )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _userContextService = userContextService ?? throw new ArgumentNullException(nameof(userContextService));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _videoPublisher = videoPublisher;

    }

    public async Task<Result<UploadVideoResult>> Handle(UploadVideoCommand command, CancellationToken cancellationToken)
    {
        //using System.Data.IDbTransaction transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            UserContext userContext = await _userContextService.GetCurrentContext(_httpContextAccessor.HttpContext);

            _logger.LogInformation("Starting video upload for user {UserId}: {FileName}",
                command.UserId, command.FileName);

            // 1. Validation
            if (command.FileSize > 5_368_709_120)
                return Result.Failure<UploadVideoResult>(
                    Error.Problem("400", "File size exceeds maximum limit of 5GB"));

            string[] allowedTypes = ["video/mp4", "video/mpeg", "video/quicktime", "video/webm"];
            if (!allowedTypes.Contains(command.ContentType.ToLower()))
                return Result.Failure<UploadVideoResult>(
                    Error.Problem("404", "Invalid video format. Allowed: MP4, MPEG, MOV, WEBM"));

            // Créer une scope pour l'opération principale
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IVideoRepository videoRepository = scope.ServiceProvider.GetRequiredService<IVideoRepository>();
            IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            IMediaStorageService storageService = scope.ServiceProvider.GetRequiredService<IMediaStorageService>();

            // 2. Créer l'entité Video
            var video = new Video
            {
                Id = Guid.CreateVersion7(),
                Title = command.Title,
                Description = command.Description,
                UserId = userContext.LocalUserId,
                FileName = command.FileName,
                FileSize = command.FileSize,
                Status = MediaStatus.Uploading,
                Tags = command.Tags ?? [],
                IsPublic = true,
            };

            await videoRepository.AddVideoAsync(video);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Video entity created with ID: {VideoId}", video.Id);

            string storagePath = string.Empty;

            try
            {
                // 3. Upload vers MinIO
                storagePath = await storageService.UploadVideoAsync(
                    video.Id, command.VideoFile, command.FileName, command.ContentType, cancellationToken);

                video.StoragePath = storagePath;

                //// 2. Générer la miniature (Opération de traitement)
                string thumbnailPath;

                if (command.ThumbnailStream != null && !string.IsNullOrEmpty(command.ThumbnailFileName))
                {
                    // Upload thumbnail fourni par l'utilisateur
                    thumbnailPath = await storageService.UploadImageAsync(
                        video.Id,
                        command.ThumbnailStream,
                        command.ThumbnailFileName,
                        MediaType.VideoThumbnail,
                        cancellationToken);

                    video.ThumbnailPath = thumbnailPath;
                }

                video.Status = MediaStatus.Processing;

                await unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Video uploaded to storage: {StoragePath}", storagePath);

                // 4. Commit transaction
                //transaction.Commit();

                //// 5. Traitement asynchrone
                _ = Task.Run(async () => await ProcessVideoAsync(video.Id,
                    video.ThumbnailPath,
                    cancellationToken), cancellationToken);
                
                //await _videoPublisher.PublishVideoForProcessingAsync(video.Id, cancellationToken);

                return Result.Success(UploadVideoResult.
                    Success(video.Id, video.Status, "Video uploaded successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload video to storage");

                // Compensation : supprimer la vidéo en base
                videoRepository.Delete(video);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                // Rollback transaction
                //transaction.Rollback();

                // Supprimer le fichier MinIO si upload partiel
                if (!string.IsNullOrEmpty(storagePath))
                {
                    try
                    {
                        await storageService.DeleteAsync(storagePath);
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogWarning(cleanupEx, "Failed to cleanup MinIO file after rollback");
                    }
                }

                return Result.Failure<UploadVideoResult>(
                    Error.Failure("500", "Failed to upload video to storage, rollback applied"));
            }
        }
        catch (Exception ex)
        {
            //transaction.Rollback();

            _logger.LogError(ex, "Unexpected error during video upload, rollback applied");
            return Result.Failure<UploadVideoResult>(
                Error.Failure("500", "Unexpected error during upload"));
        }
    }

    private async Task ProcessVideoAsync(Guid videoId,
        string thumbnailPath,
        CancellationToken cancellationToken)
    {

        // Créer une nouvelle scope pour le traitement en arrière-plan
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IVideoRepository videoRepository = scope.ServiceProvider.GetRequiredService<IVideoRepository>();
        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        IMediaStorageService storageService = scope.ServiceProvider.GetRequiredService<IMediaStorageService>();
        IMediaProcessingService processingService = scope.ServiceProvider.GetRequiredService<IMediaProcessingService>();
        IVideoQualityRepository videoQlRepository = scope.ServiceProvider.GetRequiredService<IVideoQualityRepository>();
        ILogger < UploadVideoCommandHandler> logger = scope.ServiceProvider.GetRequiredService<ILogger<UploadVideoCommandHandler>>();

        // 🚨 ATTENTION: Les variables 'video', 'duration', 'thumbnailPath' doivent 
        // être modifiables pour être mises à jour après les opérations.
        Video video;

        // --- SCOPE 1: Lecture initiale (pour obtenir l'objet Video) ---
        video = await videoRepository.GetVideoById(videoId);

        //} // Scope 1 est détruit

        if (video == null || string.IsNullOrEmpty(video.StoragePath))
        {
            _logger.LogWarning("Video not found or has no storage path: {VideoId}", videoId);
            return;
        }

        _logger.LogInformation("Starting video processing for {VideoId}", videoId);

        try
        {
            string localPath = await storageService.DownloadVideoAsync(video.StoragePath, cancellationToken);

            // 1. Extraire la durée (Opération de traitement)
            TimeSpan duration = await processingService.GetDurationAsync(localPath);
            video.Duration = duration;
            video.Status = MediaStatus.Ready;
            video.PublishedAt = DateTime.UtcNow;
            video.UpdatedAt = DateTime.UtcNow;           

            _logger.LogInformation("Video duration extracted: {Duration}", duration);

            //// 2. Générer la miniature (Opération de traitement)
            //string thumbnailPath;

            if (string.IsNullOrEmpty(thumbnailPath))
            {
                // Générer avec FFmpeg seulement si pas fourni
                thumbnailPath = await processingService.GenerateThumbnailAsync(
                    video.Id,
                    localPath); // video.StoragePath);

                video.ThumbnailPath = thumbnailPath;
            }

            await videoRepository.UpdateVideoStatus(video);

            //video.ThumbnailPath = thumbnailPath;
            _logger.LogInformation("Thumbnail generated: {ThumbnailPath}", thumbnailPath);

            //// 3. Transcoder en différentes qualités

            foreach (string quality in VideoQualitySetting.All)
            {
                try
                {
                    _logger.LogInformation(
                        "Starting transcoding to {Quality} for video {VideoId}",
                        quality,
                        videoId);

                    // 🚨 TranscodeVideoAsync est l'opération de LONGUE durée !
                    string qualityPath = await processingService.TranscodeVideoAsync(videoId, video.StoragePath, quality, cancellationToken);

                    long qualitySize = await storageService.GetFileSizeAsync(qualityPath);

                    //// --- SCOPE 2: Sauvegarde d'une qualité (après une longue attente) ---

                        var videoQuality = new VideoQuality
                        {
                            Id = Guid.CreateVersion7(),
                            VideoId = videoId,
                            Quality = quality,
                            StoragePath = qualityPath,
                            FileSize = qualitySize,
                            CreatedAt = DateTime.UtcNow
                        };

                        await videoQlRepository.AddVideoQuality(videoQuality);
                        await unitOfWork.SaveChangesAsync(cancellationToken);
                    //} // Scope 2 est détruit. Un nouveau DbContext a été créé et utilisé.
                    // -------------------------------------------------------------------

                    _logger.LogInformation(
                        "Transcoding completed for {Quality}: {QualityPath}",
                        quality,
                        qualityPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to transcode video {VideoId} to {Quality}",
                        videoId,
                        quality);
                }
            }

            _logger.LogInformation("Video processing completed successfully for {VideoId}", videoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process video {VideoId}", videoId);

            //// --- SCOPE D'ERREUR: Mise à jour du statut 'Failed' ---
            if (video != null)
            {
                video.Status = MediaStatus.Failed;
                video.UpdatedAt = DateTime.UtcNow;
                await videoRepository.UpdateVideoStatus(video); // Sauvegarde l'échec
            }
            // -----------------------------------------------------
        }
    }
}
