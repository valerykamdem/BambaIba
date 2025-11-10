using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Likes;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Likes.GetLikeStatus;
public sealed class GetLikeStatusQueryHandler : IQueryHandler<GetLikeStatusQuery, Result<GetLikeStatusResult>>
{
    private readonly ILikeRepository _likeRepository;
    private readonly IVideoRepository _videoRepository;
    private readonly IUserContextService _userContextService;
    private readonly ILogger<GetLikeStatusQueryHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public GetLikeStatusQueryHandler(
        ILikeRepository likeRepository,
        IVideoRepository videoRepository,
        IUserContextService userContextService,
        ILogger<GetLikeStatusQueryHandler> logger,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork)
    {
        _likeRepository = likeRepository ?? throw new ArgumentNullException(nameof(likeRepository));
        _videoRepository = videoRepository ?? throw new ArgumentNullException(nameof(videoRepository));
        _userContextService = userContextService ?? throw new ArgumentNullException(nameof(userContextService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<GetLikeStatusResult>> Handle(GetLikeStatusQuery query, CancellationToken cancellationToken)
    {
        try
        {

            UserContext userContext = await _userContextService.GetCurrentContext(_httpContextAccessor.HttpContext);

            _logger.LogInformation("Get User id for user {UserId}",
                userContext.LocalUserId);

            Video? video = await _videoRepository.GetVideoById(query.VideoId);

            if (video == null)
                return Result.Failure<GetLikeStatusResult>(VideoErrors.NotFound(query.VideoId));

            Like? userLike = await _likeRepository
                .GetLikeByUserAndVideoAsync(userContext.LocalUserId, video.Id, cancellationToken);


            return Result.Success(new GetLikeStatusResult
            {
                HasLiked = userLike?.IsLike == true,
                HasDisliked = userLike?.IsLike == false,
                LikeCount = video?.LikeCount ?? 0,
                DislikeCount = video?.DislikeCount ?? 0
            });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error get Like");
            return Result.Failure<GetLikeStatusResult>(LikeErrors.ErrorLikeStatus(ex.Message));
        }
    }
}
