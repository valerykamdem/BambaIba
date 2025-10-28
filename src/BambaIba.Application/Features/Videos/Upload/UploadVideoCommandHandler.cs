using BambaIba.Application.Common.Interfaces;
using BambaIba.Domain.Entities;
using BambaIba.SharedKernel.Enums;
using Cortex.Mediator.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace BambaIba.Application.Features.Videos.Upload;
public class UploadVideoCommandHandler : ICommandHandler<UploadVideoCommand, UploadVideoResult>
{
    private readonly IVideoRepository _videoRepository;
    private readonly IVideoQualityRepository _videoQlRepository;
    private readonly IVideoStorageService _storageService;
    private readonly IVideoProcessingService _processingService;
    private readonly ILogger<UploadVideoCommandHandler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public UploadVideoCommandHandler(
        IVideoRepository videoRepository,
        IVideoQualityRepository videoQlRepository,
        IVideoStorageService storageService,
        IVideoProcessingService processingService,
        ILogger<UploadVideoCommandHandler> logger,
        IServiceScopeFactory scopeFactory)
    {
        _videoRepository = videoRepository;
        _videoQlRepository = videoQlRepository;
        _storageService = storageService;
        _processingService = processingService;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }
    public async Task<UploadVideoResult> Handle(UploadVideoCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Starting video upload for user {UserId}: {FileName}",
                command.UserId,
                command.FileName);

            // 1. Validation initiale
            if (command.FileSize > 5_368_709_120) // 5GB
            {
                return UploadVideoResult.Failure("File size exceeds maximum limit of 5GB");
            }

            string[] allowedTypes = ["video/mp4", "video/mpeg", "video/quicktime", "video/webm"];
            if (!allowedTypes.Contains(command.ContentType.ToLower()))
            {
                return UploadVideoResult.Failure("Invalid video format. Allowed: MP4, MPEG, MOV, WEBM");
            }

            // 2. Créer l'entité Video
            var video = new Video
            {
                Id = Guid.CreateVersion7(),
                Title = command.Title,
                Description = command.Description,
                UserId = command.UserId,
                OriginalFileName = command.FileName,
                FileSize = command.FileSize,
                Status = VideoStatus.Uploading,
                Tags = command.Tags ?? [],
                IsPublic = true,
            };

            await _videoRepository.AddVideo(video, cancellationToken);
            //await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Video entity created with ID: {VideoId}", video.Id);

            // 3. Upload vers MinIO
            try
            {
                string storagePath = await _storageService.UploadVideoAsync(
                    video.Id,
                    command.File,
                    cancellationToken);

                video.StoragePath = storagePath;
                video.Status = VideoStatus.Processing;
                //await _context.SaveChangesAsync(cancellationToken);

                await _videoRepository.UpdateVideoStatus(video);

                _logger.LogInformation(
                    "Video uploaded to storage: {StoragePath}",
                    storagePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload video to storage");
                video.Status = VideoStatus.Failed;
                //await _context.SaveChangesAsync(cancellationToken);
                await _videoRepository.UpdateVideoStatus(video);

                return UploadVideoResult.Failure(ex.Message);// "Failed to upload video to storage");
            }

            // 4. Traitement asynchrone en arrière-plan
            _ = Task.Run(async () => await ProcessVideoAsync(video.Id), cancellationToken);

            // 5. Retourner le résultat
            return UploadVideoResult.Success(video.Id, video.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during video upload");
            return UploadVideoResult.Failure("An unexpected error occurred during upload");
        }
    }

    private async Task ProcessVideoAsync(Guid videoId)
    {
        // 🚨 ATTENTION: Les variables 'video', 'duration', 'thumbnailPath' doivent 
        // être modifiables pour être mises à jour après les opérations.
        Video video;

        // --- SCOPE 1: Lecture initiale (pour obtenir l'objet Video) ---
        using (IServiceScope scope = _scopeFactory.CreateScope())
        {
            IVideoRepository videoRepository = scope.ServiceProvider.GetRequiredService<IVideoRepository>();
            video = await videoRepository.GetVideoById(videoId);
        } // Scope 1 est détruit

        if (video == null || string.IsNullOrEmpty(video.StoragePath))
        {
            _logger.LogWarning("Video not found or has no storage path: {VideoId}", videoId);
            return;
        }

        _logger.LogInformation("Starting video processing for {VideoId}", videoId);

        try
        {
            // 1. Extraire la durée (Opération de traitement)
            TimeSpan duration = await _processingService.GetVideoDurationAsync(video.StoragePath);
            video.Duration = duration;
            _logger.LogInformation("Video duration extracted: {Duration}", duration);

            // 💡 On pourrait ajouter un scope ici pour sauvegarder la durée si l'opération est longue.
            // Pour l'exemple, nous allons grouper toutes les mises à jour finales.

            // 2. Générer la miniature (Opération de traitement)
            string thumbnailPath = await _processingService.GenerateThumbnailAsync(
                videoId,
                video.StoragePath);
            video.ThumbnailPath = thumbnailPath;
            _logger.LogInformation("Thumbnail generated: {ThumbnailPath}", thumbnailPath);

            // 3. Transcoder en différentes qualités
            string[] qualities = ["240p", "360p", "480p", "720p", "1080p"];

            foreach (string quality in qualities)
            {
                try
                {
                    _logger.LogInformation(
                        "Starting transcoding to {Quality} for video {VideoId}",
                        quality,
                        videoId);

                    // 🚨 TranscodeVideoAsync est l'opération de LONGUE durée !
                    string qualityPath = await _processingService.TranscodeVideoAsync(
                        videoId,
                        video.StoragePath,
                        quality);

                    long qualitySize = await _storageService.GetFileSizeAsync(qualityPath);

                    // --- SCOPE 2: Sauvegarde d'une qualité (après une longue attente) ---
                    using (IServiceScope innerScope = _scopeFactory.CreateScope())
                    {
                        IVideoQualityRepository videoQlRepository = innerScope.ServiceProvider.GetRequiredService<IVideoQualityRepository>();

                        var videoQuality = new VideoQuality
                        {
                            Id = Guid.NewGuid(),
                            VideoId = videoId,
                            Quality = quality,
                            StoragePath = qualityPath,
                            FileSize = qualitySize,
                            CreatedAt = DateTime.UtcNow
                        };

                        await videoQlRepository.AddVideoQuality(videoQuality);
                        // Assurez-vous que AddVideoQuality() appelle SaveChangesAsync()
                    } // Scope 2 est détruit. Un nouveau DbContext a été créé et utilisé.
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

            // 4. Marquer comme Ready (Mise à jour finale)
            // --- SCOPE 3: Sauvegarde finale des données de la vidéo ---
            using (IServiceScope finalScope = _scopeFactory.CreateScope())
            {
                IVideoRepository finalVideoRepository = 
                    finalScope.ServiceProvider.GetRequiredService<IVideoRepository>();

                // Ré-attacher la vidéo ou la recharger si nécessaire pour la mise à jour :
                // La variable 'video' est un objet de l'ancien scope, il faut le recharger 
                // ou le mettre à jour dans le nouveau contexte avant de le sauvegarder.

                // Mettre à jour l'état de l'objet récupéré précédemment
                video.Status = VideoStatus.Ready;
                video.PublishedAt = DateTime.UtcNow;
                video.UpdatedAt = DateTime.UtcNow;

                // Si votre repository gère le contexte, il doit attacher/mettre à jour cet objet.
                await finalVideoRepository.UpdateVideoStatus(video);
                // Assurez-vous que UpdateVideoStatus() gère la sauvegarde
            } // Scope 3 est détruit.
            // -------------------------------------------------------------

            _logger.LogInformation("Video processing completed successfully for {VideoId}", videoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process video {VideoId}", videoId);

            // --- SCOPE D'ERREUR: Mise à jour du statut 'Failed' ---
            using IServiceScope errorScope = _scopeFactory.CreateScope();
            IVideoRepository errorVideoRepository = errorScope.ServiceProvider.GetRequiredService<IVideoRepository>();

            // Recharger l'objet vidéo dans le nouveau contexte de l'erreur
            Video failedVideo = await errorVideoRepository.GetVideoById(videoId);
            if (failedVideo != null)
            {
                failedVideo.Status = VideoStatus.Failed;
                failedVideo.UpdatedAt = DateTime.UtcNow;
                await errorVideoRepository.UpdateVideoStatus(failedVideo); // Sauvegarde l'échec
            }
            // -----------------------------------------------------
        }
    }

    //private async Task ProcessVideoAsync(Guid videoId)
    //{
    //    try
    //    {
    //        Video video = await _videoRepository.GetVideoById(videoId);
    //        if (video == null || string.IsNullOrEmpty(video.StoragePath))
    //        {
    //            _logger.LogWarning("Video not found or has no storage path: {VideoId}", videoId);
    //            return;
    //        }

    //        _logger.LogInformation("Starting video processing for {VideoId}", videoId);

    //        // 1. Extraire la durée
    //        TimeSpan duration = await _processingService.GetVideoDurationAsync(video.StoragePath);
    //        video.Duration = duration;
    //        //await _context.SaveChangesAsync();

    //        _logger.LogInformation("Video duration extracted: {Duration}", duration);

    //        // 2. Générer la miniature
    //        string thumbnailPath = await _processingService.GenerateThumbnailAsync(
    //            videoId,
    //            video.StoragePath);
    //        video.ThumbnailPath = thumbnailPath;
    //        //await _context.SaveChangesAsync();

    //        _logger.LogInformation("Thumbnail generated: {ThumbnailPath}", thumbnailPath);

    //        // 3. Transcoder en différentes qualités
    //        string[] qualities = ["240p","360p", "480p", "720p", "1080p"];

    //        foreach (string quality in qualities)
    //        {
    //            try
    //            {
    //                _logger.LogInformation(
    //                    "Starting transcoding to {Quality} for video {VideoId}",
    //                    quality,
    //                    videoId);

    //                string qualityPath = await _processingService.TranscodeVideoAsync(
    //                    videoId,
    //                    video.StoragePath,
    //                    quality);

    //                long qualitySize = await _storageService.GetFileSizeAsync(qualityPath);

    //                var videoQuality = new VideoQuality
    //                {
    //                    Id = Guid.NewGuid(),
    //                    VideoId = videoId,
    //                    Quality = quality,
    //                    StoragePath = qualityPath,
    //                    FileSize = qualitySize,
    //                    CreatedAt = DateTime.UtcNow
    //                };

    //                await _videoQlRepository.AddVideoQuality(videoQuality);
    //                //await _context.SaveChangesAsync();

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
    //                    videoId,
    //                    quality);
    //                // Continue avec les autres qualités même si une échoue
    //            }
    //        }

    //        // 4. Marquer comme Ready
    //        video.Status = VideoStatus.Ready;
    //        video.PublishedAt = DateTime.UtcNow;
    //        video.UpdatedAt = DateTime.UtcNow;
    //        //await _context.SaveChangesAsync();

    //        _logger.LogInformation("Video processing completed successfully for {VideoId}", videoId);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Failed to process video {VideoId}", videoId);

    //        Video video = await _videoRepository.GetVideoById(videoId);
    //        if (video != null)
    //        {
    //            video.Status = VideoStatus.Failed;
    //            video.UpdatedAt = DateTime.UtcNow;
    //            //await _context.SaveChangesAsync();
    //        }
    //    }
    //}
}
