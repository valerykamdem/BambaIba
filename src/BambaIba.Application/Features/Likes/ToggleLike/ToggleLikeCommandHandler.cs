using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Features.Comments.CreateComment;
using BambaIba.Domain.Comments;
using BambaIba.Domain.Likes;
using BambaIba.Domain.MediaBase;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Likes.ToggleLike;
public sealed class ToggleLikeCommandHandler : ICommandHandler<ToggleLikeCommand, Result<ToggleLikeResult>>
{
    private readonly ILikeRepository _likeRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly IUserContextService _userContextService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ToggleLikeCommandHandler> _logger;

    public ToggleLikeCommandHandler(
        ILikeRepository likeRepository,
        IMediaRepository mediaRepository,
        IUserContextService userContextService,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        ILogger<ToggleLikeCommandHandler> logger)
    { 
        _likeRepository = likeRepository;
        _mediaRepository = mediaRepository;
        _userContextService = userContextService;
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ToggleLikeResult>> Handle(ToggleLikeCommand command, CancellationToken cancellationToken)
    {

        try
        {
            UserContext userContext = await _userContextService
                .GetCurrentContext(_httpContextAccessor.HttpContext);

            Media media = await _mediaRepository.GetMediaByIdAsync(command.MediaId, cancellationToken);

            if (media == null)
                return ToggleLikeResult.Failure("Media not found");
            
            Like existingLike = await _likeRepository
                .GetLikeByUserAndMediaAsync(
                    userContext.LocalUserId,
                    command.MediaId,
                    cancellationToken); 

            if (existingLike != null)
            {
                // User clique sur le même bouton → Retirer le like/dislike
                if (existingLike.IsLiked == command.IsLiked)
                {
                    _likeRepository.Delete(existingLike);

                    if (existingLike.IsLiked)
                        media.LikeCount--;
                    else
                        media.DislikeCount--;

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    return ToggleLikeResult.Success(
                        media.LikeCount,
                        media.DislikeCount,
                        new UserLikeStatus { HasLiked = false, HasDisliked = false });
                }
                else
                {
                    // User change d'avis (like → dislike ou vice versa)
                    if (existingLike.IsLiked)
                    {
                        media.LikeCount--;
                        media.DislikeCount++;
                    }
                    else
                    {
                        media.DislikeCount--;
                        media.LikeCount++;
                    }

                    existingLike.IsLiked = command.IsLiked;
                    existingLike.CreatedAt = DateTime.UtcNow;

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    return Result.Success(ToggleLikeResult.Success(
                        media.LikeCount,
                        media.DislikeCount,
                        new UserLikeStatus
                        {
                            HasLiked = command.IsLiked,
                            HasDisliked = !command.IsLiked
                        }));
                }
            }
            else
            {
                // Nouveau like/dislike
                var like = new Like
                {
                    Id = Guid.CreateVersion7(),
                    MediaId = command.MediaId,
                    UserId = userContext.LocalUserId,
                    IsLiked = command.IsLiked,
                    CreatedAt = DateTime.UtcNow
                };

                await _likeRepository.AddLikeAsync(like);

                if (command.IsLiked)
                    media.LikeCount++;
                else
                    media.DislikeCount++;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(ToggleLikeResult.Success(
                    media.LikeCount,
                    media.DislikeCount,
                    new UserLikeStatus
                    {
                        HasLiked = command.IsLiked,
                        HasDisliked = !command.IsLiked
                    }));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling like");
            return ToggleLikeResult.Failure("An error occurred");
        }
    }
}
