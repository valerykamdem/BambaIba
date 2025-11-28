using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Abstractions.Services;
using BambaIba.Application.Settings;
using BambaIba.Domain.Audios;
using BambaIba.Domain.Enums;
using BambaIba.Domain.MediaBase;
using BambaIba.Domain.VideoQualities;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace BambaIba.Application.Features.MediaBase.UploadMedia;

public sealed class UploadMediaCommandHandler : ICommandHandler<UploadMediaCommand, Result<UploadMediaResult>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<UploadMediaCommandHandler> _logger;
    private readonly IUserContextService _userContextService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly VideoPublisher _videoPublisher;

    public UploadMediaCommandHandler(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<UploadMediaCommandHandler> logger,
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

    public async Task<Result<UploadMediaResult>> Handle(UploadMediaCommand command, CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await _userContextService.GetCurrentContext(_httpContextAccessor.HttpContext);

            _logger.LogInformation("Starting {Type} upload for user {UserId}: {FileName}",
                command.Type, command.UserId, command.FileName);

            // 1. Validation
            Result validationResult = ValidateUploadMedia(command);
            if (validationResult.IsFailure)
                return Result.Failure<UploadMediaResult>(validationResult.Error);

            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IMediaRepository mediaRepository = scope.ServiceProvider.GetRequiredService<IMediaRepository>();
            IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            IMediaStorageService storageService = scope.ServiceProvider.GetRequiredService<IMediaStorageService>();

            // 2. Créer l'entité Audio ou Video
            Media media = command.Type.Equals("video", StringComparison.OrdinalIgnoreCase)
                ? new Video
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
                    Speaker = command.Speaker ?? string.Empty,
                    Category = command.Category ?? string.Empty,
                    Topic = command.Topic ?? string.Empty,
                    Qualities = []
                }
                : command.Type.Equals("audio", StringComparison.OrdinalIgnoreCase)
                    ? (Media)new Audio
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
                        Speaker = command.Speaker ?? string.Empty,
                        Category = command.Category ?? string.Empty,
                        Topic = command.Topic ?? string.Empty
                    }
                    : throw new InvalidOperationException("Type de média inconnu. Utilisez 'audio' ou 'video'.");

            await mediaRepository.AddMediaAsync(media);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("{Type} entity created with ID: {MediaId}", command.Type, media.Id);

            string storagePath = string.Empty;

            try
            {
                // 3. Upload vers MinIO
                if (command.Type.Equals("video", StringComparison.OrdinalIgnoreCase))
                {
                    storagePath = await storageService.UploadVideoAsync(
                        media.Id, command.MediaFile, command.FileName, command.ContentType, cancellationToken);

                    // Miniature pour vidéo
                    if (command.ThumbnailStream != null && !string.IsNullOrEmpty(command.ThumbnailFileName))
                    {
                        string thumbnailPath = await storageService.UploadImageAsync(
                            media.Id,
                            command.ThumbnailStream,
                            command.ThumbnailFileName,
                            MediaType.VideoThumbnail,
                            cancellationToken);

                        media.ThumbnailPath = thumbnailPath;
                    }
                }
                else if (command.Type.Equals("audio", StringComparison.OrdinalIgnoreCase))
                {
                    storagePath = await storageService.UploadAudioAsync(
                        media.Id, command.MediaFile, command.FileName, command.ContentType, cancellationToken);

                    // Miniature pour audio
                    if (command.ThumbnailStream != null && !string.IsNullOrEmpty(command.ThumbnailFileName))
                    {
                        string thumbnailPath = await storageService.UploadImageAsync(
                            media.Id,
                            command.ThumbnailStream,
                            command.ThumbnailFileName,
                            MediaType.AudioCover,
                            cancellationToken);

                        media.ThumbnailPath = thumbnailPath;
                    }
                }

                media.StoragePath = storagePath;
                media.Status = MediaStatus.Processing;

                await unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("{Type} uploaded to storage: {StoragePath}", command.Type, storagePath);

                // 4. Traitement asynchrone
                _ = Task.Run(async () => await ProcessMediaAsync(media.Id, media.ThumbnailPath, cancellationToken), cancellationToken);

                return Result.Success(UploadMediaResult.Success(
                    media.Id, media.Status, $"{command.Type} uploaded successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload {Type} to storage", command.Type);

                // Compensation : supprimer en base
                await mediaRepository.DeleteAsync(media);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                if (!string.IsNullOrEmpty(storagePath))
                {
                    try
                    { await storageService.DeleteAsync(storagePath); }
                    catch (Exception cleanupEx) { _logger.LogWarning(cleanupEx, "Failed to cleanup storage after rollback"); }
                }

                return Result.Failure<UploadMediaResult>(
                    Error.Failure("500", $"Failed to upload {command.Type} to storage, rollback applied"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during {Type} upload, rollback applied", command.Type);
            return Result.Failure<UploadMediaResult>(
                Error.Failure("500", "Unexpected error during upload"));
        }
    }


    //public async Task<Result<UploadMediaResult>> Handle(UploadMediaCommand command, CancellationToken cancellationToken)
    //{
    //    //using System.Data.IDbTransaction transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

    //    try
    //    {
    //        UserContext userContext = await _userContextService.GetCurrentContext(_httpContextAccessor.HttpContext);

    //        _logger.LogInformation("Starting video upload for user {UserId}: {FileName}",
    //            command.UserId, command.FileName);

    //        //// 1. Validation

    //        Result validationResult = ValidateUploadMedia(command);
    //        if (validationResult.IsFailure)
    //        {
    //            return Result.Failure<UploadMediaResult>(validationResult.Error);
    //        }

    //        // Créer une scope pour l'opération principale
    //        using IServiceScope scope = _serviceScopeFactory.CreateScope();
    //        IMediaRepository mediaRepository = scope.ServiceProvider.GetRequiredService<IMediaRepository>();
    //        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    //        IMediaStorageService storageService = scope.ServiceProvider.GetRequiredService<IMediaStorageService>();

    //        // 2. Créer l'entité Video ou Audio en base de données
    //        Media media = command.Type.Equals("video", StringComparison.OrdinalIgnoreCase)
    //            ? new Video
    //            {
    //                Id = Guid.CreateVersion7(),
    //                Title = command.Title,
    //                Description = command.Description,
    //                UserId = userContext.LocalUserId,
    //                FileName = command.FileName,
    //                FileSize = command.FileSize,
    //                Status = MediaStatus.Uploading,
    //                Tags = command.Tags ?? [],
    //                IsPublic = true,
    //                Qualities = [] // initialisation
    //            }
    //            : command.Type.Equals("audio", StringComparison.OrdinalIgnoreCase)
    //                ? (Media)new Audio
    //                {
    //                    Id = Guid.CreateVersion7(),
    //                    Title = command.Title,
    //                    Description = command.Description,
    //                    UserId = userContext.LocalUserId,
    //                    FileName = command.FileName,
    //                    FileSize = command.FileSize,
    //                    Status = MediaStatus.Uploading,
    //                    Tags = command.Tags ?? [],
    //                    IsPublic = true,
    //                    Speaker = command.Speaker ?? string.Empty,
    //                    Category = command.Category ?? string.Empty,
    //                    Topic = command.Topic ?? string.Empty
    //                }
    //                : throw new InvalidOperationException("Type de média inconnu. Utilisez 'audio' ou 'video'.");

    //        await mediaRepository.AddAsync(media);
    //        await unitOfWork.SaveChangesAsync(cancellationToken);

    //        _logger.LogInformation("Video entity created with ID: {MediaId}", media.Id);

    //        string storagePath = string.Empty;

    //        try
    //        {
    //            // 3. Upload vers MinIO
    //            if (command.Type.Equals("video", StringComparison.OrdinalIgnoreCase))

    //                storagePath = await storageService.UploadVideoAsync(
    //                    media.Id, command.MediaFile, command.FileName, command.ContentType, cancellationToken);

    //            media.StoragePath = storagePath;

    //            //// 2. Générer la miniature (Opération de traitement)
    //            string thumbnailPath;

    //            if (command.ThumbnailStream != null && !string.IsNullOrEmpty(command.ThumbnailFileName))
    //            {
    //                // Upload thumbnail fourni par l'utilisateur
    //                thumbnailPath = await storageService.UploadImageAsync(
    //                    media.Id,
    //                    command.ThumbnailStream,
    //                    command.ThumbnailFileName,
    //                    MediaType.VideoThumbnail,
    //                    cancellationToken);

    //                media.ThumbnailPath = thumbnailPath;
    //            }

    //            media.Status = MediaStatus.Processing;

    //            await unitOfWork.SaveChangesAsync(cancellationToken);

    //            _logger.LogInformation("Video uploaded to storage: {StoragePath}", storagePath);

    //            // 4. Commit transaction
    //            //transaction.Commit();

    //            //// 5. Traitement asynchrone
    //            _ = Task.Run(async () => await ProcessMediaAsync(media.Id,
    //                media.ThumbnailPath,
    //                cancellationToken), cancellationToken);

    //            //await _videoPublisher.PublishVideoForProcessingAsync(video.Id, cancellationToken);

    //            return Result.Success(UploadMediaResult.
    //                Success(media.Id, media.Status, "Video uploaded successfully"));
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Failed to upload video to storage");

    //            // Compensation : supprimer la vidéo en base
    //            await mediaRepository.DeleteAsync(media);
    //            await unitOfWork.SaveChangesAsync(cancellationToken);

    //            // Rollback transaction
    //            //transaction.Rollback();

    //            // Supprimer le fichier MinIO si upload partiel
    //            if (!string.IsNullOrEmpty(storagePath))
    //            {
    //                try
    //                {
    //                    await storageService.DeleteAsync(storagePath);
    //                }
    //                catch (Exception cleanupEx)
    //                {
    //                    _logger.LogWarning(cleanupEx, "Failed to cleanup MinIO file after rollback");
    //                }
    //            }

    //            return Result.Failure<UploadMediaResult>(
    //                Error.Failure("500", "Failed to upload video to storage, rollback applied"));
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        //transaction.Rollback();

    //        _logger.LogError(ex, "Unexpected error during video upload, rollback applied");
    //        return Result.Failure<UploadMediaResult>(
    //            Error.Failure("500", "Unexpected error during upload"));
    //    }
    //}

    private Result<UploadMediaResult> ValidateUploadMedia(UploadMediaCommand command)
    {
        // Vérifier le titre
        if (string.IsNullOrWhiteSpace(command.Title))
        {
            return Result.Failure<UploadMediaResult>(
                Error.Problem("400", "Title is required"));
        }

        // Normaliser le ContentType
        string contentType = command.ContentType?.ToLower() ?? string.Empty;

        if (command.Type.Equals("audio", StringComparison.OrdinalIgnoreCase))
        {
            // Taille max : 200MB
            if (command.FileSize > 209_715_200)
            {
                return Result.Failure<UploadMediaResult>(
                    Error.Problem("400", "Audio file exceeds maximum size of 200MB"));
            }

            // Formats audio autorisés
            string[] allowedFormats = { "audio/mpeg", "audio/mp3", "audio/wav", "audio/ogg", "audio/aac", "audio/flac" };
            if (!allowedFormats.Contains(contentType))
            {
                return Result.Failure<UploadMediaResult>(
                    Error.Problem("415", "Invalid audio format. Allowed: MP3, WAV, OGG, AAC, FLAC"));
            }
        }
        else if (command.Type.Equals("video", StringComparison.OrdinalIgnoreCase))
        {
            // Taille max : 5GB
            if (command.FileSize > 5_368_709_120)
            {
                return Result.Failure<UploadMediaResult>(
                    Error.Problem("400", "Video file exceeds maximum size of 5GB"));
            }

            // Formats vidéo autorisés
            string[] allowedFormats = { "video/mp4", "video/mpeg", "video/quicktime", "video/webm" };
            if (!allowedFormats.Contains(contentType))
            {
                return Result.Failure<UploadMediaResult>(
                    Error.Problem("415", "Invalid video format. Allowed: MP4, MPEG, MOV, WEBM"));
            }
        }
        else
        {
            return Result.Failure<UploadMediaResult>(
                Error.Problem("400", "Unknown media type. Must be 'audio' or 'video'"));
        }

        // ✅ Tout est valide
        return Result.Success(new UploadMediaResult());
    }

    private async Task ProcessMediaAsync(Guid mediaId, string? thumbnailPath, CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
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


    //private async Task ProcessVideoAsync(Guid mediaId,
    //    string thumbnailPath, CancellationToken cancellationToken)
    //{

    //    // Créer une nouvelle scope pour le traitement en arrière-plan
    //    using IServiceScope scope = _serviceScopeFactory.CreateScope();
    //    IMediaRepository mediaRepository = scope.ServiceProvider.GetRequiredService<IMediaRepository>();
    //    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    //    IMediaStorageService storageService = scope.ServiceProvider.GetRequiredService<IMediaStorageService>();
    //    IMediaProcessingService processingService = scope.ServiceProvider.GetRequiredService<IMediaProcessingService>();
    //    IVideoQualityRepository videoQlRepository = scope.ServiceProvider.GetRequiredService<IVideoQualityRepository>();
    //    ILogger<UploadMediaCommandHandler> logger = scope.ServiceProvider.GetRequiredService<ILogger<UploadMediaCommandHandler>>();

    //    // 🚨 ATTENTION: Les variables 'video', 'duration', 'thumbnailPath' doivent 
    //    // être modifiables pour être mises à jour après les opérations.
    //    Media media;

    //    // --- SCOPE 1: Lecture initiale (pour obtenir l'objet Video) ---
    //    media = await mediaRepository.GetByIdAsync(mediaId, cancellationToken);

    //    //} // Scope 1 est détruit

    //    if (media == null || string.IsNullOrEmpty(media.StoragePath))
    //    {
    //        _logger.LogWarning("Video not found or has no storage path: {VideoId}", mediaId);
    //        return;
    //    }

    //    _logger.LogInformation("Starting video processing for {VideoId}", mediaId);

    //    try
    //    {
    //        string localPath = await storageService.DownloadVideoAsync(media.StoragePath, cancellationToken);

    //        // 1. Extraire la durée (Opération de traitement)
    //        TimeSpan duration = await processingService.GetDurationAsync(localPath);
    //        media.Duration = duration;
    //        media.Status = MediaStatus.Ready;
    //        media.PublishedAt = DateTime.UtcNow;
    //        media.UpdatedAt = DateTime.UtcNow;

    //        _logger.LogInformation("Video duration extracted: {Duration}", duration);

    //        //// 2. Générer la miniature (Opération de traitement)
    //        //string thumbnailPath;

    //        if (string.IsNullOrEmpty(thumbnailPath))
    //        {
    //            // Générer avec FFmpeg seulement si pas fourni
    //            thumbnailPath = await processingService.GenerateThumbnailAsync(
    //                media.Id,
    //                localPath); // video.StoragePath);

    //            media.ThumbnailPath = thumbnailPath;
    //        }

    //        await mediaRepository.UpdateMediaStatus(media);

    //        //video.ThumbnailPath = thumbnailPath;
    //        _logger.LogInformation("Thumbnail generated: {ThumbnailPath}", thumbnailPath);

    //        //// 3. Transcoder en différentes qualités

    //        foreach (string quality in VideoQualitySetting.All)
    //        {
    //            try
    //            {
    //                _logger.LogInformation(
    //                    "Starting transcoding to {Quality} for video {VideoId}",
    //                    quality,
    //                    mediaId);

    //                // 🚨 TranscodeVideoAsync est l'opération de LONGUE durée !
    //                string qualityPath = await processingService.TranscodeVideoAsync(mediaId, media.StoragePath, quality, cancellationToken);

    //                long qualitySize = await storageService.GetFileSizeAsync(qualityPath);

    //                //// --- SCOPE 2: Sauvegarde d'une qualité (après une longue attente) ---

    //                var videoQuality = new VideoQuality
    //                {
    //                    Id = Guid.CreateVersion7(),
    //                    VideoId = mediaId,
    //                    Quality = quality,
    //                    StoragePath = qualityPath,
    //                    FileSize = qualitySize,
    //                    CreatedAt = DateTime.UtcNow
    //                };

    //                await videoQlRepository.AddVideoQuality(videoQuality);
    //                await unitOfWork.SaveChangesAsync(cancellationToken);
    //                //} // Scope 2 est détruit. Un nouveau DbContext a été créé et utilisé.
    //                // -------------------------------------------------------------------

    //                _logger.LogInformation(
    //                    "Transcoding completed for {Quality}: {QualityPath}",
    //                    quality,
    //                    qualityPath);
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(
    //                    ex,
    //                    "Failed to transcode video {VideoId} to {Quality}",
    //                    mediaId,
    //                    quality);
    //            }
    //        }

    //        _logger.LogInformation("Video processing completed successfully for {VideoId}", mediaId);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Failed to process video {VideoId}", mediaId);

    //        //// --- SCOPE D'ERREUR: Mise à jour du statut 'Failed' ---
    //        if (media != null)
    //        {
    //            media.Status = MediaStatus.Failed;
    //            media.UpdatedAt = DateTime.UtcNow;
    //            await mediaRepository.UpdateMediaStatus(media); // Sauvegarde l'échec
    //        }
    //        // -----------------------------------------------------
    //    }
    //}
}
