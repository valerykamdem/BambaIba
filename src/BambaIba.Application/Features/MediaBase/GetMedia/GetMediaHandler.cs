using BambaIba.Application.Abstractions.Caching;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.Videos;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.MediaBase.GetMedia;

public sealed record GetMediaQuery(
    string? Cursor = null,
    int? Limit = null,
    string? Search = null);

public class GetMediaHandler(
    IBIDbContext dbContext,
    ElasticsearchClient elasticClient, // Injected Elasticsearch client (Concrete class)
    ICacheService cacheService,
    IMediaStorageService mediaStorageService,
    IUserContextService userContextService,
    ILogger<GetMediaHandler> logger)
{
    private const int DefaultLimit = 20;
    private const int MaxLimit = 50;
    // Cache TTL reduced for search results or disabled if real-time is critical
    private const int CacheTtlMinutes = 2;

    public async Task<Result<CursorPagedResult<MediaDto>>> Handle(
        GetMediaQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get current user context
            UserContext userContext = await userContextService.GetCurrentContext();
            Guid? currentUserId = userContext?.LocalUserId;

            int effectiveLimit = Math.Min(query.Limit ?? DefaultLimit, MaxLimit);

            // 2. Branching logic: Search (Elastic) vs Feed (Postgres)
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                // Use Elasticsearch for text search
                return await SearchFromElasticAsync(query, currentUserId, effectiveLimit, cancellationToken);
            }
            else
            {
                // Use Postgres for standard feed browsing (Cursor Pagination)
                return await GetFeedFromPostgresAsync(query, currentUserId, effectiveLimit, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving media");
            return SharedKernel.Result.Failure<CursorPagedResult<MediaDto>>(
                Error.Failure("500", "An error occurred while retrieving media"));
        }
    }

    // =======================================================
    // STRATEGY 1: Elasticsearch Search (Fast Text Search)
    // =======================================================
    private async Task<Result<CursorPagedResult<MediaDto>>> SearchFromElasticAsync(
        GetMediaQuery query,
        Guid? currentUserId,
        int limit,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Searching Elastic for: {Search}", query.Search);

        // 1. Query Elasticsearch
        // Note: We use 'search_after' for cursor pagination with ES, or simple From/Size for simplicity here.
        // For a V1, we will use simple pagination logic, but return a structure compatible with the app.
        SearchResponse<MediaDocument> searchResponse = await elasticClient.SearchAsync<MediaDocument>(s => s
            .Indices("media_index")
            .Size(limit + 1) // Fetch one extra to determine HasNextPage
            .Query(q => q
                .MultiMatch(mm => mm
                    .Fields(new[] { "title^3", "speaker^2", "description", "tags", "category" }) // Boost title
                    .Query(query.Search)
                    .Fuzziness("AUTO") // Handle typos
                )
            )
            .Sort(sort => sort
                .Field(f => f.Field("_score").Order(SortOrder.Desc)) // Relevance first
                .Field(f => f.Field(p => p.PublishedAt).Order(SortOrder.Desc)) // Then date
            ), cancellationToken);

        if (!searchResponse.IsValidResponse)
        {
            logger.LogError("Elasticsearch error: {Error}", searchResponse.DebugInformation);
            // Failover: Return empty or throw, depending on policy
            return SharedKernel.Result.Failure<CursorPagedResult<MediaDto>>(Error.Failure("Search.Error", "Search engine unavailable"));
        }

        var elasticIds = searchResponse.Documents.Select(d => d.Id).ToList();
        bool hasNextPage = elasticIds.Count > limit;

        if (hasNextPage)
        {
            elasticIds.RemoveAt(elasticIds.Count - 1); // Remove the extra item
        }

        if (!elasticIds.Any())
        {
            return SharedKernel.Result.Success(new CursorPagedResult<MediaDto>([], null, false));
        }

        // 2. Hydrate from Postgres (Batch loading details, stats, reactions)
        // We ignore the incoming 'Cursor' for search for now, as ES handles its own pagination
        List<MediaAsset> dbMedia = await dbContext.MediaAssets
            .Include(m => m.Stat)
            .Where(m => elasticIds.Contains(m.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // 3. Mapping & Reaction Logic
        // IMPORTANT: We must preserve the order from Elasticsearch (Relevance), not DB order
        var dtos = new List<MediaDto>();
        foreach (Guid id in elasticIds)
        {
            MediaAsset? item = dbMedia.FirstOrDefault(m => m.Id == id);
            if (item == null)
                continue;

            // Check user reaction (N+1 potential here, usually better to batch load reactions)
            // For simplicity, keeping the logic similar to original
            string? userReaction = currentUserId.HasValue
                ? await dbContext.MediaReactions
                    .Where(r => r.MediaId == id && r.UserId == currentUserId)
                    .Select(r => r.ReactionType.ToString())
                    .FirstOrDefaultAsync(cancellationToken)
                : null;

            dtos.Add(MapToDto(item, userReaction, mediaStorageService));
        }

        // Note: Cursor for Search is tricky (requires encoding ES sort values). 
        // Simplified here to null for this iteration.
        return SharedKernel.Result.Success(new CursorPagedResult<MediaDto>(dtos, null, hasNextPage));
    }

    // =======================================================
    // STRATEGY 2: Postgres Feed (Cursor Pagination)
    // =======================================================
    private async Task<Result<CursorPagedResult<MediaDto>>> GetFeedFromPostgresAsync(
        GetMediaQuery query,
        Guid? currentUserId,
        int limit,
        CancellationToken ct)
    {
        // Check Cache
        string cacheKey = BuildCacheKey(query, limit);
        if (await cacheService.GetAsync<CursorPagedResult<MediaDto>>(cacheKey, ct) is { } cached)
        {
            return SharedKernel.Result.Success(cached);
        }

        // Build Query
        IQueryable<MediaAsset> mediaQuery = dbContext.MediaAssets
            .Include(m => m.Stat)
            .AsNoTracking()
            .Where(m => m.Status == MediaStatus.Ready && m.IsPublic);

        // Apply Cursor Filtering
        CursorData? cursorData = CursorExtensions.Decode<CursorData>(query.Cursor);
        if (cursorData is not null)
        {
            mediaQuery = mediaQuery.Where(m => m.CreatedAt < cursorData.CreatedAt);
        }

        // Fetch Data
        int takePlusOne = limit + 1;

        // Projection with User Reaction
        var projectedItems = await mediaQuery
            .OrderByDescending(m => m.CreatedAt)
            .ThenBy(m => m.Id)
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

                // Reaction Logic
                UserReactionType = currentUserId != null
                    ? m.Reactions
                        .Where(r => r.UserId == currentUserId)
                        .Select(r => r.ReactionType.ToString())
                        .FirstOrDefault()
                    : null
            })
            .ToListAsync(ct);

        // Pagination Logic
        bool hasNextPage = projectedItems.Count == takePlusOne;
        if (hasNextPage)
            projectedItems.RemoveAt(projectedItems.Count - 1);

        // Mapping
        var dtoItems = projectedItems.Select(item => new MediaDto
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            Speaker = item.Speaker,
            Category = item.Category,
            Language = item.Language,
            ThumbnailUrl = mediaStorageService.GetPublicUrl(BucketType.Image, item.ThumbnailPath),
            Duration = item.Duration,
            PlayCount = item.Stat?.PlayCount ?? 0,
            LikeCount = item.Stat?.LikeCount ?? 0,
            DislikeCount = item.Stat?.DislikeCount ?? 0,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            UserId = item.UserId,
            Type = item.Discriminator.ToLower(),
            CommentCount = item.Stat?.CommentCount ?? 0,
            IsPublic = item.IsPublic,
            PublishedAt = item.PublishedAt,
            Tags = item.Tags,
            IsLiked = item.UserReactionType == ReactionType.Like.ToString(),
            IsDisliked = item.UserReactionType == ReactionType.Dislike.ToString(),
        }).ToList();

        // Next Cursor
        string? nextCursor = dtoItems.Count > 0
            ? CursorExtensions.Encode(new CursorData((DateTime)dtoItems.Last().CreatedAt!, dtoItems.Last().Id))
            : null;

        var result = new CursorPagedResult<MediaDto>(dtoItems, nextCursor, hasNextPage);

        // Set Cache
        await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CacheTtlMinutes), ct);

        return SharedKernel.Result.Success(result);
    }

    // Helper for mapping single entity (Used in Elastic path)
    private static MediaDto MapToDto(MediaAsset m, string? userReactionType, IMediaStorageService storageService)
    {
        return new MediaDto
        {
            Id = m.Id,
            Title = m.Title,
            Description = m.Description,
            Speaker = m.Speaker,
            Category = m.Category,
            Language = m.Language,
            ThumbnailUrl = storageService.GetPublicUrl(BucketType.Image, m.ThumbnailPath),
            Duration = m.Duration,
            PlayCount = m.Stat?.PlayCount ?? 0,
            LikeCount = m.Stat?.LikeCount ?? 0,
            DislikeCount = m.Stat?.DislikeCount ?? 0,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt,
            UserId = m.UserId,
            Type = m is Video ? "video" : "audio", // Simple discriminator logic
            CommentCount = m.Stat?.CommentCount ?? 0,
            IsPublic = m.IsPublic,
            PublishedAt = m.PublishedAt,
            Tags = m.Tags,
            IsLiked = userReactionType == ReactionType.Like.ToString(),
            IsDisliked = userReactionType == ReactionType.Dislike.ToString()
        };
    }

    private static string BuildCacheKey(GetMediaQuery query, int effectiveLimit)
    {
        return $"media:cursor:{query.Cursor ?? "first"}|{effectiveLimit}|{query.Search ?? "all"}";
    }
}
