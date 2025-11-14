using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities;
using BambaIba.Domain.Enums;
using BambaIba.Domain.VideoQualities;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Videos;
using Cortex.Mediator.Queries;
using Mapster;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Videos.GetVideoById;
public sealed class GetVideoByIdQueryHandler : IQueryHandler<GetVideoByIdQuery, Result<VideoWithQualitiesResult>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly IVideoQualityRepository _videoQualityRepository;
    private readonly IMediaStorageService _mediaStorageService;
    private readonly ILogger<GetVideoByIdQueryHandler> _logger;

    public GetVideoByIdQueryHandler(
        IVideoRepository videoRepository,
        IVideoQualityRepository videoQualityRepository,
        IMediaStorageService mediaStorageService,
    ILogger<GetVideoByIdQueryHandler> logger)
    {
        _videoRepository = videoRepository;
        _videoQualityRepository = videoQualityRepository;
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

            IQueryable<VideoQuality> videoQualities = _videoQualityRepository.GetAllByVideoId(query.VideoId);
           
            _logger.LogInformation("Successfully retrieved video with ID: {VideoId}", query.VideoId);

            var qualities = videoQualities.Select(q => new VideoQualityDto
            {               
                Quality = q.Quality,
                //StoragePath = q.StoragePath,
                PublicUrl = _mediaStorageService.GetPublicUrl(BucketType.Video, q.StoragePath)
            }).ToList();

            return Result.Success(new VideoWithQualitiesResult
            {
                Id = video.Id,
                Title = video.Title,
                Description = video.Description,
                VideoUrl = _mediaStorageService.GetPublicUrl(BucketType.Video, video.StoragePath),
                ThumbnailUrl = _mediaStorageService.GetPublicUrl(BucketType.Image, video.ThumbnailPath),
                Duration = video.Duration,
                ViewCount = video.ViewCount,
                LikeCount = video.LikeCount,
                DislikeCount = video.DislikeCount,
                Qualities = qualities,
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
