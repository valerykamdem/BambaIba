using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.Playlists;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Playlists.GetPlaylistDetails;

public sealed record GetPlaylistDetailsQuery(Guid PlaylistId);

public sealed class GetPlaylistDetailsHandler(
    IBIDbContext dbContext,
    IMediaStorageService storageService,
    IUserContextService userContextService
    /*ILogger<GetPlaylistDetailsHandler> logger*/)
{
    public async Task<Result<PlaylistDetailsDto>> Handle(
        GetPlaylistDetailsQuery query,
        CancellationToken cancellationToken)
    {
        UserContext userContext = await userContextService.GetCurrentContext();

        // 1. Get Playlist + Check Access
        Playlist? playlist = await dbContext.Playlists
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == query.PlaylistId, cancellationToken);

        if (playlist == null)
            return Result.Failure<PlaylistDetailsDto>(Error.NotFound("Not.found", "Playlist not found"));

        // Security: Private playlists are only visible to owner
        if (playlist.Visibility == PlaylistVisibility.Private && playlist.UserId != userContext.LocalUserId)
        {
            return Result.Failure<PlaylistDetailsDto>(Error.Forbidden("Access.Denied", "This playlist is private."));
        }

        // 2. Get Medias
        var mediaIds = playlist.Items.Select(i => i.MediaId).ToList();

        List<MediaAsset> medias = await dbContext.MediaAssets
            .Include(m => m.Stat)
            .AsNoTracking()
            .Where(m => mediaIds.Contains(m.Id) && m.Status == MediaStatus.Ready)
            .ToListAsync(cancellationToken);

        // 3. Order Medias according to playlist position
        // We use a Join to preserve the order defined in PlaylistItems
        var orderedMedias = medias
            .Join(playlist.Items,
                  m => m.Id,
                  i => i.MediaId,
                  (m, i) => new { Media = m, Item = i })
            .OrderBy(x => x.Item.Position)
            .Select(x => MapToMediaDto(x.Media, storageService))
            .ToList();

        var result = new PlaylistDetailsDto(
            playlist.Id,
            playlist.Name,
            playlist.Description,
            playlist.MediaCount,
            playlist.ThumbnailUrl,
            orderedMedias
        );

        return Result.Success(result);
    }

    // CORRECTION: Use Object Initializer syntax { } instead of Constructor ()
    private MediaDto MapToMediaDto(MediaAsset m, IMediaStorageService storage)
    {
        return new MediaDto
        {
            Id = m.Id,
            Title = m.Title,
            Description = m.Description,
            Speaker = m.Speaker,
            Category = m.Category,
            Language = m.Language,
            ThumbnailUrl = storage.GetPublicUrl(BucketType.Image, m.ThumbnailPath),
            Duration = m.Duration,
            PlayCount = m.Stat?.PlayCount ?? 0,
            LikeCount = m.Stat?.LikeCount ?? 0,
            DislikeCount = m.Stat?.DislikeCount ?? 0,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt,
            UserId = m.UserId,
            Type = EF.Property<string>(m, "Discriminator").ToLower(), // "video" or "audio"
            CommentCount = m.Stat?.CommentCount ?? 0,
            IsPublic = m.IsPublic,
            PublishedAt = m.PublishedAt,
            Tags = m.Tags,

            // Default values for reactions in this context (we didn't load user reactions)
            IsLiked = false,
            IsDisliked = false
        };
    }
}
