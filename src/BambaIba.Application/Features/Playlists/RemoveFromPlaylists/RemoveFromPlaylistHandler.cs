
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.PlaylistItems;
using BambaIba.Domain.Entities.Playlists;
using BambaIba.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace BambaIba.Application.Features.Playlists.RemoveFromPlaylists;

public sealed record RemoveFromPlaylistCommand(Guid PlaylistId, Guid MediaId);

public sealed class RemoveFromPlaylistHandler(
    IBIDbContext dbContext,
    IUserContextService userContextService
    /*ILogger<RemoveFromPlaylistHandler> logger*/)
{
    public async Task<Result> Handle(RemoveFromPlaylistCommand cmd, CancellationToken ct)
    {
        UserContext user = await userContextService.GetCurrentContext();

        PlaylistItem? item = await dbContext.PlaylistItems
            .Where(pi => pi.PlaylistId == cmd.PlaylistId && pi.MediaId == cmd.MediaId)
            .FirstOrDefaultAsync(ct);

        if (item == null)
            return Result.Failure(Error.NotFound("Not.found", "Item not found"));

        // Vérifier que l'utilisateur est le propriétaire de la playlist
        Playlist? playlist = await dbContext.Playlists.FindAsync(new object[] { cmd.PlaylistId }, ct);
        if (playlist == null || playlist.UserId != user.LocalUserId)
            return Result.Failure(Error.Forbidden("Not.Allow", "not Allow"));

        dbContext.PlaylistItems.Remove(item);

        // Mise à jour du compteur (simplement décrémenter)
        playlist.MediaCount--;

        // Note: Ici on ne met pas à jour la ThumbnailUrl pour garder le code simple.
        // En prod, si tu supprimes le 1er item, il faudrait trouver le nouveau 1er pour mettre à jour l'image.

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
