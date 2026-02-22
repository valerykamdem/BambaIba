using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.Users;
using BambaIba.Domain.Entities.Videos;
using BambaIba.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.MediaBase.DeleteMedia;

public sealed record DeleteMediaCommand(Guid MediaId);

public sealed class DeleteMediaHandler
{
    public static async Task<Result<DeleteMediaResult>> Handle(
        DeleteMediaCommand command,
        IBIDbContext dbContext,
        IMediaStorageService storageService,
        IUserContextService userContextService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DeleteMediaHandler> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await userContextService.GetCurrentContext(httpContextAccessor.HttpContext);

            MediaAsset media = await dbContext.MediaAssets.FindAsync(command.MediaId, cancellationToken);

            if (media == null)
                return Result.Failure<DeleteMediaResult>(VideoErrors.NotFound(command.MediaId));

           if(media.UserId != userContext.LocalUserId)
                return Result.Failure<DeleteMediaResult>(UserErrors.NotFound(command.MediaId));

            // Supprimer les fichiers associés de stockage
            await storageService.DeleteAsync(media.Id.ToString(), media.StoragePath);
            logger.LogInformation("Deleting video files from storage for VideoId: {VideoId}", media.StoragePath);

            // Supprimer la vidéo de la base de données seulement si l'utilisateur est le propriétaire
            dbContext.MediaAssets.Remove(media);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Deleting video files from storage for VideoId: {VideoId}", command.MediaId);

            return Result.Success(DeleteMediaResult.Success(media.Id));

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting video with ID: {VideoId}", command.MediaId);
            throw;
        }

    }
}
