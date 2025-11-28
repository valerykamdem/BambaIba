using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Extensions;
using BambaIba.Domain.Audios;
using BambaIba.Domain.Enums;
using BambaIba.Domain.MediaBase;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.MediaBase.GetMedia;

public sealed class GetMediaQueryHandler : IQueryHandler<GetMediaQuery, Result<PagedResult<MediaDto>>>
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IMediaStorageService _mediaStorageService;
    private readonly ILogger<GetMediaQueryHandler> _logger;

    public GetMediaQueryHandler(
        IMediaRepository mediaRepository,
        IMediaStorageService mediaStorageService,
    ILogger<GetMediaQueryHandler> logger)
    {
        _mediaRepository = mediaRepository;
        _mediaStorageService = mediaStorageService;
        _logger = logger;
    }

    public async Task<Result<PagedResult<MediaDto>>> Handle(GetMediaQuery query, CancellationToken cancellationToken)
    {
        try
        {
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

            return Result.Success(new PagedResult<MediaDto>
            {
                Items = pagedResult.Items,
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving videos");
            throw;
        }
    }
}
