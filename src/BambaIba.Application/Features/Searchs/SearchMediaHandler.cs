using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Searchs;

public record SearchMediaQuery(string Term, int Limit = 20);

public class SearchMediaHandler(
    IBIDbContext dbContext,
    ElasticsearchClient elasticClient,
    IMediaStorageService storageService,
    ILogger<SearchMediaHandler> logger)
{
    public async Task<Result<CursorPagedResult<MediaDto>>> Handle(SearchMediaQuery query, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query.Term))
        {
            return SharedKernel.Result.Success(new CursorPagedResult<MediaDto>([], null, false));
        }

        try
        {
            // 1. Recherche dans Elasticsearch
            SearchResponse<MediaDocument> searchResponse = await elasticClient.SearchAsync<MediaDocument>(s => s
                .Indices("media_index")
                .Size(query.Limit + 1)
                .Query(q => q
                    .MultiMatch(mm => mm
                        .Fields(new[] { "title^3", "speaker^2", "description", "tags", "category" })
                        .Query(query.Term)
                        .Fuzziness("AUTO")
                    )
                ), ct);

            if (!searchResponse.IsValidResponse)
            {
                logger.LogError("Elasticsearch error: {Error}", searchResponse.DebugInformation);
                return SharedKernel.Result.Failure<CursorPagedResult<MediaDto>>(Error.Failure("Search.Error", "Search engine error"));
            }

            var elasticIds = searchResponse.Documents.Select(d => d.Id).ToList();
            bool hasNextPage = elasticIds.Count > query.Limit;
            if (hasNextPage)
                elasticIds.RemoveAt(elasticIds.Count - 1);

            if (!elasticIds.Any())
            {
                return SharedKernel.Result.Success(new CursorPagedResult<MediaDto>(new List<MediaDto>(), null, false));
            }

            // 2. Hydratation depuis Postgres (pour avoir les Stats, Réactions, etc.)
            List<MediaAsset> dbMedia = await dbContext.MediaAssets
                .Include(m => m.Stat)
                .Where(m => elasticIds.Contains(m.Id) && m.Status == MediaStatus.Ready)
                .AsNoTracking()
                .ToListAsync(ct);

            // 3. Mapping vers DTO
            // Note: On conserve l'ordre de pertinence d'Elasticsearch
            var dtos = elasticIds.Join(dbMedia,
                id => id,
                db => db.Id,
                (id, media) => MapToDto(media, storageService))
                .ToList();

            return SharedKernel.Result.Success(new CursorPagedResult<MediaDto>(dtos, null, hasNextPage));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Search failed");
            return SharedKernel.Result.Failure<CursorPagedResult<MediaDto>>(Error.Failure("Search.Exception", ex.Message));
        }
    }

    private static MediaDto MapToDto(MediaAsset m, IMediaStorageService storage)
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
            Type = m is Domain.Entities.Videos.Video ? "video" : "audio",
            CommentCount = m.Stat?.CommentCount ?? 0,
            IsPublic = m.IsPublic,
            PublishedAt = m.PublishedAt,
            Tags = m.Tags,
            IsLiked = false, // Pas de contexte user dans cette version simple
            IsDisliked = false
        };
    }
}
