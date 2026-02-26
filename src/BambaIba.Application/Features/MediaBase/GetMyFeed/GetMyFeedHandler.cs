using BambaIba.Application.Abstractions.Caching;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace BambaIba.Application.Features.MediaBase.GetMyFeed;

public sealed record GetMyFeedQuery(
    string? Cursor = null,
    int? Limit = null
);

public sealed class GetMyFeedHandler(
    IBIDbContext dbContext,
    //ICacheService cacheService,
    IMediaStorageService mediaStorageService,
    IUserContextService userContextService)
{
    private const int DefaultLimit = 20;
    private const int MaxLimit = 50;

    public async Task<Result<CursorPagedResult<MediaDto>>> Handle(
        GetMyFeedQuery query,
        CancellationToken cancellationToken)
    {

        // 1. Récupérer l'utilisateur connecté
        UserContext userContext = await userContextService
            .GetCurrentContext();

        //Guid? currentUserId = userContext?.LocalUserId;
        if (userContext?.LocalUserId is null)
        {
            return Result.Failure<CursorPagedResult<MediaDto>>(Error.Failure("Not.Authen", "Not Authorization"));
        }

        // 2. Récupérer la liste des IDs des gens que l'utilisateur suit
        List<Guid> subscribedChannelIds = await dbContext.UserSubscriptions
            .Where(s => s.FollowerId == userContext.LocalUserId)
            .Select(s => s.ChannelId)
            .ToListAsync(cancellationToken);


        //List<Guid> followingIds = await dbContext.UserSubscriptions
        //    .Where(s => s.FollowerId == userContext.LocalUserId)
        //    .Select(s => s.FollowingId)
        //    .ToListAsync(cancellationToken);

        if (!subscribedChannelIds.Any())
        {
            // Si l'utilisateur ne suit personne, on retourne une liste vide
            return Result.Success(new CursorPagedResult<MediaDto>([], "", false));
        }

        // 3. Construire la requête
        IOrderedQueryable<MediaAsset> mediaQuery = dbContext.MediaAssets        
            .Include(m => m.Stat)
            .Include(m => m.Reactions)
            .AsNoTracking()
            .Where(m => subscribedChannelIds.Contains(m.ChannelId))
            .OrderByDescending(m => m.PublishedAt);

        // 4. Application du Curseur (Identique à avant)
        CursorData? cursorData = CursorExtensions.Decode<CursorData>(query.Cursor);
        if (cursorData is not null)
        {
            mediaQuery = (IOrderedQueryable<MediaAsset>)mediaQuery.Where(m => m.CreatedAt < cursorData.CreatedAt);
        }

        int effectiveLimit = Math.Min(query.Limit ?? DefaultLimit, MaxLimit);
        int takePlusOne = effectiveLimit + 1;

        // 5. Récupération et Projection (Identique à GetMediaHandler)
        var mediaItems = await mediaQuery
            .Take(takePlusOne)
            .Select(m => new
            {
                m.Id,
                m.Title,
                m.Description,
                m.Speaker,
                m.Category,
                m.Language,
                m.ThumbnailPath,
                m.Duration,
                m.CreatedAt,
                m.UpdatedAt,
                m.UserId,
                m.IsPublic,
                m.PublishedAt,
                m.Tags,
                m.Stat,
                Discriminator = EF.Property<string>(m, "Discriminator"),
                UserReactionType = m.Reactions
                    .Where(r => r.UserId == userContext.LocalUserId)
                    .Select(r => r.ReactionType)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        // 7. Gestion de la Pagination
        bool hasNextPage = mediaItems.Count == takePlusOne;

        if (hasNextPage)
        {
            mediaItems.RemoveAt(mediaItems.Count - 1);
        }

        // 8. Mapping vers MediaDto
        var dtoItems = mediaItems.Select(item => new MediaDto
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            Speaker = item.Speaker,
            Category = item.Category,
            Language = item.Language,
            ThumbnailUrl = mediaStorageService.GetPublicUrl(BucketType.Image, item.ThumbnailPath),
            Duration = item.Duration,
            PlayCount = item.Stat != null ? item.Stat.PlayCount : 0,
            LikeCount = item.Stat != null ? item.Stat.LikeCount : 0,
            DislikeCount = item.Stat != null ? item.Stat.DislikeCount : 0,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            UserId = item.UserId,
            Type = item.Discriminator.ToLower(),
            CommentCount = item.Stat != null ? item.Stat.CommentCount : 0,
            IsPublic = item.IsPublic,
            PublishedAt = item.PublishedAt,
            Tags = item.Tags,

            // MAPPING DES RÉACTIONS
            IsLiked = item.UserReactionType == ReactionType.Like,
            IsDisliked = item.UserReactionType == ReactionType.Dislike
        }).ToList();

        // 9. Génération du Curseur Suivant
        string? nextCursor = dtoItems.Count > 0
            ? CursorExtensions.Encode(new CursorData((DateTime)dtoItems.Last().CreatedAt!, dtoItems.Last().Id))
            : null;

        var result = new CursorPagedResult<MediaDto>
        (
            dtoItems,
            nextCursor,
            hasNextPage
        );

        //await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), cancellationToken);
        return Result.Success(result);
    }

    //private static string BuildCacheKey(GetMediaQuery query, int effectiveLimit)
    //{
    //    var factors = new List<string>
    //    {
    //        $"cursor:{query.Cursor ?? "first"}",
    //        $"limit:{effectiveLimit}",
    //        $"search:{query.Search ?? "all"}"
    //    };

    //    return $"media:cursor:{string.Join("|", factors)}";
    //}

}
