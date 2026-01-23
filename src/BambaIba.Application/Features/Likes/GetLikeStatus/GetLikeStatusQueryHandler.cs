using BambaIba.Domain.Entities.Likes;
using BambaIba.Domain.Entities.MediaBase;
using BambaIba.Domain.Entities.Videos;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Likes.GetLikeStatus;
public sealed class GetLikeStatusQueryHandler : IQueryHandler<GetLikeStatusQuery, Result<GetLikeStatusResult>>
{
    private readonly IMediaRepository _mediaRepository;
    private readonly ILogger<GetLikeStatusQueryHandler> _logger;

    public GetLikeStatusQueryHandler(
        IMediaRepository mediaRepository,
        ILogger<GetLikeStatusQueryHandler> logger)
    {
        _mediaRepository = mediaRepository ?? throw new ArgumentNullException(nameof(mediaRepository));      
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<GetLikeStatusResult>> Handle(GetLikeStatusQuery query, CancellationToken cancellationToken)
    {
        try
        {

            Media? media = await _mediaRepository.GetMediaByIdAsync(query.MediaId, cancellationToken);

            if (media == null)
                return Result.Failure<GetLikeStatusResult>(VideoErrors.NotFound(query.MediaId));

            return Result.Success(new GetLikeStatusResult
            {
                //HasLiked = userLike?.IsLike == true,
                //HasDisliked = userLike?.IsLike == false,
                LikeCount = media?.LikeCount ?? 0,
                DislikeCount = media?.DislikeCount ?? 0
            });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error get Like");
            return Result.Failure<GetLikeStatusResult>(LikeErrors.ErrorLikeStatus(ex.Message));
        }
    }
}
