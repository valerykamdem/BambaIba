using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.PlaylistItems;
using BambaIba.Domain.Entities.Playlists;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Playlists.AddMediaToPlaylists;

public sealed record AddMediaToPlaylistCommand(Guid PlaylistId, Guid MediaId);

public sealed class AddMediaToPlaylistHandler(
    IBIDbContext dbContext,
    IMediaStorageService storageService,
    IUserContextService userContextService,
    ILogger<AddMediaToPlaylistHandler> logger)
{

    public async Task<Result> Handle(
        AddMediaToPlaylistCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await userContextService.GetCurrentContext();

            if (userContext == null)
                return Result.Failure<Guid>(Error.Unauthorized("401", "User not authenticated"));

            // 1. Vérifier que la playlist appartient à l'utilisateur
            Playlist? playlist = await dbContext.Playlists
                .FirstOrDefaultAsync(p => p.Id == command.PlaylistId && p.UserId == userContext.LocalUserId, cancellationToken);

            if (playlist == null)
                return Result.Failure(Error.NotFound("Not.found", "Playlist not found"));

            // 2. Vérifier si le média n'est pas déjà dedans
            bool exists = await dbContext.PlaylistItems
                .AnyAsync(pi => pi.PlaylistId == command.PlaylistId && pi.MediaId == command.MediaId, cancellationToken);

            if (exists)
                return Result.Failure(Error.Conflict("Already.Exist", "Media already in playlist"));

            // 3. Trouver la prochaine position (Count + 1)
            int nextPosition = await dbContext.PlaylistItems
                .Where(pi => pi.PlaylistId == command.PlaylistId)
                .CountAsync(cancellationToken) + 1;

            // 4. Récupérer l'URL de la miniature du média
            // On a besoin du media pour ça. Pour l'instant on fait une petite requête.
            MediaAsset? media = await dbContext.MediaAssets.FindAsync([command.MediaId], cancellationToken);

            // 5. Ajouter l'item
            dbContext.PlaylistItems.Add(new PlaylistItem
            {
                Id = Guid.CreateVersion7(),
                PlaylistId = command.PlaylistId,
                MediaId = command.MediaId,
                Position = nextPosition
            });

            // 6. Mise à jour de la playlist (Compteur + Thumbnail si c'est le 1er)
            playlist.MediaCount++;
            if (playlist.MediaCount == 1 && media != null)
            {
                playlist.ThumbnailUrl = storageService.GetPublicUrl(BucketType.Image, media.ThumbnailPath);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Media {MediaId} added to playlist {PlaylistId}",
                command.MediaId, command.PlaylistId);

            return Result.Success($"Media {command.MediaId} added to playlist {command.PlaylistId}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding Media to playlist");
            return Result.Failure(Error.Failure("An error occurred", ex.Message));
        }
    }
}
