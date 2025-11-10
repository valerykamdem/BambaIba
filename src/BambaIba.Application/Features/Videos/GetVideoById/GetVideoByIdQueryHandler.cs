using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Videos;
using Cortex.Mediator.Queries;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Videos.GetVideoById;
public sealed class GetVideoByIdQueryHandler : IQueryHandler<GetVideoByIdQuery, Result<VideoDetailResult>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly IVideoStorageService _storageService;
    private readonly ILogger<GetVideoByIdQueryHandler> _logger;

    public GetVideoByIdQueryHandler(
        IVideoRepository videoRepository,
        IVideoStorageService storageService,
        ILogger<GetVideoByIdQueryHandler> logger)
    {
        _videoRepository = videoRepository;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<Result<VideoDetailResult>> Handle(GetVideoByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving video with ID: {VideoId}", query.VideoId);
            VideoDetailResult videoDetail = await _videoRepository.GetVideoDetailResultById(query.VideoId, cancellationToken);
            if (videoDetail == null)
            {
                _logger.LogWarning("Video with ID: {VideoId} not found", query.VideoId);
                return null;
            }
            _logger.LogInformation("Successfully retrieved video with ID: {VideoId}", query.VideoId);
            return Result.Success<VideoDetailResult>(new VideoDetailResult
            {
                Id = videoDetail.Id,
                Title = videoDetail.Title,
                Description = videoDetail.Description,
                VideoUrl = videoDetail.VideoUrl,
                ThumbnailUrl = videoDetail.ThumbnailUrl,
                Duration = videoDetail.Duration,
                ViewCount = videoDetail.ViewCount,
                LikeCount = videoDetail.LikeCount,
                DislikeCount = videoDetail.DislikeCount,
                Qualities = videoDetail.Qualities,
                CreatedAt = videoDetail.CreatedAt,
                UserId = videoDetail.UserId,
                CommentCount = videoDetail.CommentCount

            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving video with ID: {VideoId}", query.VideoId);
            throw;
        }
    }

}
