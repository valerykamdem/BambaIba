using System.Text;
using BambaIba.Application.Abstractions.Caching;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.MediaBase.GetMedia;

public sealed record GetMediaQuery(
    string? Cursor = null,
    int? Limit = null,
    string? Search = null);

public class GetMediaHandler(IBIDbContext dbContext,
        ICacheService cacheService,
        IMediaStorageService mediaStorageService,
        ILogger<GetMediaHandler> logger)
{

    // Define application defaults and boundaries
    private const int DefaultLimit = 20;
    private const int MaxLimit = 50;
    private const int CacheTtlMinutes = 2;

    public async Task<Result<CursorPagedResult<MediaDto>>> Handle(
        GetMediaQuery query,
        CancellationToken cancellationToken)
    {

        try
        {
            // 1. Déterminer la limite effective
            int effectiveLimit = Math.Min(query.Limit ?? DefaultLimit, MaxLimit);

            // 2. Créer une clé de cache intelligente
            string cacheKey = BuildCacheKey(query, effectiveLimit);

            if (await cacheService.GetAsync<CursorPagedResult<MediaDto>>(cacheKey, cancellationToken) is { } cached)
            {
                logger.LogInformation("Retrieved media from cache with cursor={Cursor}", query.Cursor);
                return Result.Success(cached);
            }

            logger.LogInformation("Fetching media with cursor={Cursor}, limit={Limit}, search={Search}", query.Cursor, query.Limit, query.Search);

            // 4. Construire la requête avec curseur
            IQueryable<MediaAsset> mediaQuery = BuildMediaQuery(dbContext, query);

            // Cursor decoding
            CursorData? cursorData = CursorExtensions.Decode<CursorData>(query.Cursor);
            if (cursorData is not null)
            {
                mediaQuery = mediaQuery.Where(m => m.CreatedAt < cursorData.CreatedAt);
            }

            // Fetch items
            int takePlusOne = effectiveLimit + 1;
            List<MediaDto>? mediaItems = await mediaQuery
                .OrderByDescending(m => m.CreatedAt)
                .ThenBy(m => m.Id)
                .Take(takePlusOne)
                .AsNoTracking()
                .Select(m => new MediaDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    Speaker = m.Speaker,
                    Category = m.Category,
                    Language = m.Language,
                    ThumbnailUrl = mediaStorageService.GetPublicUrl(BucketType.Image, m.ThumbnailPath),
                    Duration = m.Duration,
                    PlayCount = m.Stat != null ? m.Stat.PlayCount : 0,
                    LikeCount = m.Stat != null ? m.Stat.LikeCount : 0,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    UserId = m.UserId,
                    Type = EF.Property<string>(m, "Discriminator").ToLower(),
                    CommentCount = m.Stat != null ? m.Stat.CommentCount : 0,
                    //DislikeCount = v.DislikeCount,
                    IsPublic = m.IsPublic,
                    PublishedAt = m.PublishedAt,
                    Tags = m.Tags,
                }).ToListAsync(cancellationToken);

            // --- LOGIQUE CORRIGÉE ---

            // 1. Déterminer si on a une page suivante
            // Si le nombre d'items récupérés est égal à ce qu'on a demandé (takePlusOne), alors il y a une suite.
            bool hasNextPage = mediaItems.Count == takePlusOne;

            // 2. Si on a une page suivante, on supprime le dernier élément (le "peek")
            // car c'était juste pour vérifier qu'il y avait quelque chose après.
            if (hasNextPage)
            {
                mediaItems.RemoveAt(mediaItems.Count - 1);
            }

            // Generate next cursor
            string? nextCursor = mediaItems.Count > 0
                ? CursorExtensions.Encode(new CursorData((DateTime)mediaItems.Last().CreatedAt, mediaItems.Last().Id))
                : null;

            var result = new CursorPagedResult<MediaDto>
            ( 
                mediaItems, 
                nextCursor,
                mediaItems.Count == takePlusOne
            );
            
            await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), cancellationToken);

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Erreur lors de la récupération des médias: Cursor={Cursor}, Limit={Limit}",
                query.Cursor, query.Limit);
            return Result.Failure<CursorPagedResult<MediaDto>>(
                Error.Failure("404",
                "Erreur lors de la récupération des médias: Cursor={Cursor}, Limit={Limit}"));
        }
    }


    // ===== MÉTHODES PRIVÉES =====

    private static IQueryable<MediaAsset> BuildMediaQuery(
        IBIDbContext dbContext,
        GetMediaQuery query)
    {
        IQueryable<MediaAsset> baseQuery = dbContext.MediaAssets
            .Include(m => m.Stat)
            .AsNoTracking()
            .Where(m => m.Status == MediaStatus.Ready && m.IsPublic)
            .OrderByDescending(m => m.PublishedAt).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            baseQuery = baseQuery.Where(m =>
                (m.Title ?? "").Contains(query.Search) ||
                (m.Speaker ?? "").Contains(query.Search) ||
                (m.Topic ?? "").Contains(query.Search));
        }

        return baseQuery;
    }

    private static string BuildCacheKey(GetMediaQuery query, int effectiveLimit)
    {
        // Clé de cache multi-facteurs pour éviter les collisions
        var factors = new List<string>
        {
            $"cursor:{query.Cursor ?? "first"}",
            $"limit:{effectiveLimit}",
            $"search:{query.Search ?? "all"}"
        };

        return $"media:cursor:{string.Join("|", factors)}";
    }
}
