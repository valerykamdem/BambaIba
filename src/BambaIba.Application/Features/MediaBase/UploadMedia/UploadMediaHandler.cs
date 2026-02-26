using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Features.MediaBase.ProcessMedia;
using BambaIba.Domain.Entities.Audios;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.Roles;
using BambaIba.Domain.Entities.Users;
using BambaIba.Domain.Entities.Videos;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wolverine;
using Wolverine.Runtime;


namespace BambaIba.Application.Features.MediaBase.UploadMedia;

public sealed class UploadMediaRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Speaker { get; set; }
    public string? Category { get; set; }
    public string? Topic { get; set; }
    public List<string>? Tags { get; set; } = new List<string>();
    public IFormFile MediaFile { get; set; } = default!;
    public IFormFile? ThumbnailFile { get; set; }
}


public sealed record UploadMediaCommand(
    string? Title,
    string? Description,

    string? Speaker,
    string? Category,
    string? Topic,
    List<string>? Tags,

    Stream MediaStream,
    string MediaFileName,
    string MediaContentType,

    Stream? ThumbnailStream,
    string? ThumbnailFileName,
    string? ThumbnailContentType
);


public sealed class UploadMediaHandler(IBIDbContext dbContext,
        IMessageBus bus,
        ILogger<UploadMediaHandler> logger,
        IUserContextService userContextService,
        //IHttpContextAccessor httpContextAccessor,
        IMediaStorageService storageService)
{

    public async Task<Result<UploadMediaResult>> Handle(
        UploadMediaCommand command,
        CancellationToken cancellationToken)
    {

        UserContext userContext = await userContextService.GetCurrentContext();
        var mediaId = Guid.CreateVersion7();

        try
        {
            // 1. Verify Role
            if (userContext.Role != RoleNames.Creator && userContext.Role != RoleNames.Admin)
            {
                return Result.Failure<UploadMediaResult>(
                    Error.Forbidden("Access.Denied", "Access Denied: Only Creators can upload content.")
                );
            }

            // 1. Déduire type + métadonnées
            string contentType = command.MediaContentType;
            long fileSize = command.MediaStream.Length;

            string mediaType = contentType.StartsWith("video")
                ? "video"
                : contentType.StartsWith("audio")
                ? "audio"
                : throw new InvalidOperationException("Unsupported media type");

            // 2. Upload vers Stockage (Seaweed/S3) - Fait avant la DB pour éviter d'enregistrer des orphelins
            string storagePath;
            string? thumbnailPath = string.Empty;

            if (mediaType.Equals("video", StringComparison.OrdinalIgnoreCase))//if (command.Type.Equals("video", StringComparison.OrdinalIgnoreCase))
            {
                storagePath = await storageService.UploadVideoAsync(mediaId, command.MediaStream,
                    command.MediaFileName, contentType, cancellationToken);

                if (command.ThumbnailStream != null)
                {
                    thumbnailPath = await storageService.UploadImageAsync(mediaId, command.ThumbnailStream,
                        command.ThumbnailFileName, MediaType.VideoThumbnail, cancellationToken);
                }
            }
            else // Audio
            {
                storagePath = await storageService.UploadAudioAsync(mediaId, command.MediaStream,
                    command.MediaFileName, contentType, cancellationToken);

                if (command.ThumbnailStream != null)
                {
                    thumbnailPath = await storageService.UploadImageAsync(mediaId, command.ThumbnailStream,
                        command.ThumbnailFileName, MediaType.AudioCover, cancellationToken);
                }
            }

            // 2. Génération automatique des données manquantes
            string autoTitle = Path.GetFileNameWithoutExtension(command.MediaFileName) ?? "Untitled Media";

            // Nettoyage du nom de fichier pour le titre (remplacer _ par des espaces, etc.)
            autoTitle = System.Text.RegularExpressions.Regex.Replace(autoTitle, "[_-]", " ");

            // 3. Création de l'entité
            MediaAsset media = mediaType == "video" //command.Type.Equals("video", StringComparison.OrdinalIgnoreCase)
                ? new Video
                {
                    Id = mediaId,
                    Title = autoTitle,
                    Description = "No description provided", //command.Description,
                    UserId = userContext.LocalUserId,
                    FileName = command.MediaFileName,
                    FileSize = fileSize,
                    StoragePath = storagePath,
                    ThumbnailPath = thumbnailPath,
                    Status = MediaStatus.Processing, // Statut initial
                    Tags = command.Tags ?? [],
                    IsPublic = true,
                    Speaker = command.Speaker ?? string.Empty,
                    Category = command.Category ?? string.Empty,
                    Topic = command.Topic ?? string.Empty,
                }
                : new Audio
                {
                    Id = mediaId,
                    Title = autoTitle,
                    Description = "No description provided",  //command.Description,
                    UserId = userContext.LocalUserId,
                    FileName = command.MediaFileName,
                    FileSize = fileSize,
                    StoragePath = storagePath,
                    ThumbnailPath = thumbnailPath,
                    Status = MediaStatus.Processing,
                    Tags = command.Tags ?? [],
                    IsPublic = true,
                    Speaker = command.Speaker ?? string.Empty,
                    Category = command.Category ?? string.Empty,
                    Topic = command.Topic ?? string.Empty,
                };

            dbContext.MediaAssets.Add(media);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Media {MediaId} uploaded successfully. Triggering processing...", mediaId);

            //// 4. Déclenchement du traitement asynchrone via Wolverine
            //// Cela remplace le Task.Run. Wolverine gère la réexécution en cas d'erreur et les scopes.
            await bus.PublishAsync(new ProcessMediaCommand(mediaId));

            return Result.Success(new UploadMediaResult()
            {
                IsSuccess = true,
                MediaId = mediaId, 
                Status = media.Status,
                Messaqge = "Upload successful, processing started",
                ErrorMessage = string.Empty,
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to upload media");
            // Gestion du rollback (si nécessaire) serait ici, mais l'upload est fait avant la DB donc moins critique à rollbacker
            return Result.Failure<UploadMediaResult>(Error.Problem("500", ex.Message + "Upload failed"));
        }
    }
}
