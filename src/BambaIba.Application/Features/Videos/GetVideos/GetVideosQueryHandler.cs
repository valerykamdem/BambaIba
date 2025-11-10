using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Videos;
using Cortex.Mediator.Queries;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Videos.GetVideos;
public sealed class GetVideosQueryHandler : IQueryHandler<GetVideosQuery, Result<GetVideosResult>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly ILogger<GetVideosQueryHandler> _logger;

    public GetVideosQueryHandler(
        IVideoRepository videoRepository,
        ILogger<GetVideosQueryHandler> logger)
    {
        _videoRepository = videoRepository;
        _logger = logger;
    }

    public async Task<Result<GetVideosResult>> Handle(GetVideosQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Getting videos: Page={Page}, PageSize={PageSize}, Search={Search}",
                query.Page,
                query.PageSize,
                query.Search);

            GetVideosResult videos = await _videoRepository.GetVideos(
                query.Page, query.PageSize, 
                query.Search, cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} videos out of {Total}",
                videos.TotalCount, videos.TotalPages);

            return Result.Success(videos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving videos");
            throw;
        }
    }
}
