
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace BambaIba.Application.Features.Playlists.GetUserPlaylist;

public sealed record GetUserPlaylistsQuery();

// DTO Simplifié pour la liste
public sealed record PlaylistDto(
    Guid Id,
    string Name,
    int MediaCount,
    string? ThumbnailUrl,
    DateTime? CreatedAt
);

public sealed class GetUserPlaylistsHandler(
    IBIDbContext dbContext,
    IUserContextService userContextService
    /*ILogger<GetUserPlaylistsHandler> logger*/)
{

    public async Task<Result<List<PlaylistDto>>> Handle(/*GetUserPlaylistsQuery query,*/ CancellationToken ct)
    {
        UserContext user = await userContextService.GetCurrentContext();

        if (user == null)
            return Result.Failure<List<PlaylistDto>>(Error.Forbidden("Unauthorized", "Unauthorized"));

        List<PlaylistDto> playlists = await dbContext.Playlists
            .AsNoTracking()
            .Where(p => p.UserId == user.LocalUserId)
            .Select(p => new PlaylistDto(
                p.Id,
                p.Name,
                p.MediaCount,
                p.ThumbnailUrl,
                p.CreatedAt
            ))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

        return Result.Success(playlists);
    }
}
