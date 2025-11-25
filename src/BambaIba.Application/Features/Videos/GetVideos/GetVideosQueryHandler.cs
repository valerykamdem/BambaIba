using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Extensions;
using BambaIba.Domain.Enums;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Videos;
using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Videos.GetVideos;
public sealed class GetVideosQueryHandler : IQueryHandler<GetVideosQuery, Result<PagedResult<VideoDto>>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly IMediaStorageService _mediaStorageService;
    private readonly ILogger<GetVideosQueryHandler> _logger;

    public GetVideosQueryHandler(
        IVideoRepository videoRepository,
        IMediaStorageService mediaStorageService,
    ILogger<GetVideosQueryHandler> logger)
    {
        _videoRepository = videoRepository;
        _mediaStorageService = mediaStorageService;
        _logger = logger;
    }

    public async Task<Result<PagedResult<VideoDto>>> Handle(GetVideosQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Getting videos: Page={Page}, PageSize={PageSize}, Search={Search}",
                query.Page,
                query.PageSize,
                query.Search);

            IQueryable<Video> videos = _videoRepository.GetVideos();

            if (!string.IsNullOrWhiteSpace(query.Search))
                videos = videos.Where(a =>
                    (a.Title ?? "").Contains(query.Search));

            PagedResult<VideoDto> pagedResult = await videos
                .Select(v => new VideoDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    Description = v.Description,
                    ThumbnailUrl = _mediaStorageService.GetPublicUrl(BucketType.Image, v.ThumbnailPath),  // ← Juste le chemin, pas le fichier
                    Duration = v.Duration,
                    ViewCount = v.PlayCount,
                    LikeCount = v.LikeCount,
                    CreatedAt = (DateTime)v.CreatedAt!,
                    UserId = v.UserId,
                })
                .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

            int totalCount = await videos.CountAsync(cancellationToken);

            return Result.Success(new PagedResult<VideoDto>
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
