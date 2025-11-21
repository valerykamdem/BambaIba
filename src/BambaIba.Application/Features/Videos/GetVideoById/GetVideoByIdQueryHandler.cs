using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Enums;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Videos;
using Cortex.Mediator.Queries;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Videos.GetVideoById;

public sealed class GetVideoByIdQueryHandler : IQueryHandler<GetVideoByIdQuery, Result<VideoWithQualitiesResult>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly IMediaStorageService _mediaStorageService;
    private readonly ILogger<GetVideoByIdQueryHandler> _logger;

    public GetVideoByIdQueryHandler(
        IVideoRepository videoRepository,
        IMediaStorageService mediaStorageService,
    ILogger<GetVideoByIdQueryHandler> logger)
    {
        _videoRepository = videoRepository;
        _mediaStorageService = mediaStorageService;
        _logger = logger;
    }

    public async Task<Result<VideoWithQualitiesResult>> Handle(GetVideoByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving video with ID: {VideoId}", query.VideoId);
            Video video = await _videoRepository.GetVideoWithQualitiesAsync(query.VideoId, cancellationToken);

            if (video == null)
            {
                _logger.LogWarning("Video with ID: {VideoId} not found", query.VideoId);
                return null;
            }

            return Result.Success(new VideoWithQualitiesResult
            {
                Id = video.Id,
                Title = video.Title,
                Description = video.Description,
                VideoUrl = _mediaStorageService.GetPublicUrl(BucketType.Video, video.StoragePath),
                ThumbnailUrl = _mediaStorageService.GetPublicUrl(BucketType.Image, video.ThumbnailPath),
                Duration = video.Duration,
                ViewCount = video.PlayCount,
                LikeCount = video.LikeCount,
                DislikeCount = video.DislikeCount,
                Qualities = [.. video.Qualities
                    .Select(q => new VideoQualityDto
                    {
                        Quality = q.Quality,
                        VideoUrl = _mediaStorageService.GetPublicUrl(BucketType.Video, q.StoragePath)
                    })],
                CreatedAt = video.CreatedAt,
                UserId = video.UserId,
                CommentCount = video.CommentCount

            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving video with ID: {VideoId}", query.VideoId);
            throw;
        }
    }

}
