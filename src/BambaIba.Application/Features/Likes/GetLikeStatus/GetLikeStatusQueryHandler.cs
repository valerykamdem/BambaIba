using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Likes;
using BambaIba.Domain.MediaBase;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Likes.GetLikeStatus;
public sealed class GetLikeStatusQueryHandler : IQueryHandler<GetLikeStatusQuery, Result<GetLikeStatusResult>>
{
    private readonly ILikeRepository _likeRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly IUserContextService _userContextService;
    private readonly ILogger<GetLikeStatusQueryHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public GetLikeStatusQueryHandler(
        ILikeRepository likeRepository,
        IMediaRepository mediaRepository,
        IUserContextService userContextService,
        ILogger<GetLikeStatusQueryHandler> logger,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork)
    {
        _likeRepository = likeRepository ?? throw new ArgumentNullException(nameof(likeRepository));
        _mediaRepository = mediaRepository ?? throw new ArgumentNullException(nameof(mediaRepository));
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

            Media? media = await _mediaRepository.GetMediaByIdAsync(query.MediaId, cancellationToken);

            if (media == null)
                return Result.Failure<GetLikeStatusResult>(VideoErrors.NotFound(query.MediaId));

            Like? userLike = await _likeRepository
                .GetLikeByUserAndVideoAsync(userContext.LocalUserId, media.Id, cancellationToken);


            return Result.Success(new GetLikeStatusResult
            {
                HasLiked = userLike?.IsLike == true,
                HasDisliked = userLike?.IsLike == false,
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
