using BambaIba.Application.Abstractions.Caching;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Extensions;
using BambaIba.Domain.Enums;
using BambaIba.Domain.MediaBase;
using BambaIba.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.MediaBase.GetMedia;

public sealed record GetMediaQuery(
    int Page,
    int PageSize,
    string? Search);

public sealed class GetMediaQueryHandler
{
    private readonly IMediaRepository _mediaRepository;
    private readonly ICacheService _cacheService;
    private readonly IMediaStorageService _mediaStorageService;
    private readonly ILogger<GetMediaQueryHandler> _logger;

    public GetMediaQueryHandler(
        IMediaRepository mediaRepository,
        ICacheService cacheService,
        IMediaStorageService mediaStorageService,
    ILogger<GetMediaQueryHandler> logger)
    {
        _mediaRepository = mediaRepository;
        _cacheService = cacheService;
        _mediaStorageService = mediaStorageService;
        _logger = logger;
    }

    public async Task<Result<PagedResult<MediaDto>>> Handle(GetMediaQuery query, CancellationToken cancellationToken)
    {
        try
        {
            if (await _cacheService.GetAsync<PagedResult<MediaDto>>($"GetMediaQuery-{query.Page}-{query.PageSize}", cancellationToken) is { } cachedResult)
            {
                _logger.LogInformation(
                    "Retrieved media from cache: Page={Page}, PageSize={PageSize}, Search={Search}",
                    query.Page,
                    query.PageSize,
                    query.Search);
                return Result.Success(cachedResult);
            }

            _logger.LogInformation(
                "Getting videos: Page={Page}, PageSize={PageSize}, Search={Search}",
                query.Page,
                query.PageSize,
                query.Search);

            IQueryable<Media> media = _mediaRepository.GetMediaAsync();

            if (!string.IsNullOrWhiteSpace(query.Search))
                media = media.Where(m =>
                    (m.Title ?? "").Contains(query.Search) ||
                    (m.Speaker ?? "").Contains(query.Search) ||
                    (m.Topic ?? "").Contains(query.Search));

            PagedResult<MediaDto> pagedResult = await media
                .Select(v => new MediaDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    Description = v.Description,
                    Speaker = v.Speaker,
                    Category = v.Category,
                    Language = v.Language,
                    ThumbnailUrl = _mediaStorageService.GetPublicUrl(BucketType.Image, v.ThumbnailPath),  // ← Juste le chemin, pas le fichier
                    Duration = v.Duration,
                    PlayCount = v.PlayCount,
                    LikeCount = v.LikeCount,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt,
                    UserId = v.UserId,
                    //Type = v is Video ? "video" : "audio",
                    Type = EF.Property<string>(v, "Discriminator").ToLower(),
                    CommentCount = v.CommentCount,
                    DislikeCount = v.DislikeCount,
                    IsPublic = v.IsPublic,
                    PublishedAt = v.PublishedAt,
                    Tags = v.Tags,
                })
                .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

            int totalCount = await media.CountAsync(cancellationToken);

            var newPagedResult = new PagedResult<MediaDto>
            {
                Items = pagedResult.Items,
                TotalCount = totalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages
            };

            await _cacheService.SetAsync($"GetMediaQuery-{query.Page}-{query.PageSize}", newPagedResult, TimeSpan.FromMinutes(5), cancellationToken);

            return Result.Success(newPagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving videos");
            throw;
        }
    }
}
