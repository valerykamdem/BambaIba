using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Comments;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Comments.CreateComment;
public sealed class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand, Result<CreateCommentResult>>
{
    private readonly ICommentRepository _commentRepository; 
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CreateCommentCommandHandler> _logger;

    public CreateCommentCommandHandler(
        ICommentRepository commentRepository,
        IVideoRepository videoRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<CreateCommentCommandHandler> logger)
    {
        _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
        _videoRepository = videoRepository ?? throw new ArgumentNullException(nameof(videoRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userContextService = userContextService ?? throw new ArgumentNullException(nameof(userContextService));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<CreateCommentResult>> Handle(CreateCommentCommand command, CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await _userContextService
                .GetCurrentContext(_httpContextAccessor.HttpContext);

            Video video = await _videoRepository.GetVideoById(command.VideoId);

            if (video == null)
                return Result.Failure<CreateCommentResult>(VideoErrors.NotFound(command.VideoId));

            // Validation : Parent comment existe ? (si réponse)
            if (command.ParentCommentId.HasValue)
            {
                bool parentExists = await _commentRepository.GetParentComment(command.ParentCommentId.Value, cancellationToken);

                if (!parentExists)
                    return Result.Failure<CreateCommentResult>(CommentErrors.NotFoundParent);
            }

            // Créer le commentaire
            var comment = new Comment
            {
                Id = Guid.CreateVersion7(),
                VideoId = command.VideoId,
                UserId = userContext.LocalUserId,
                Content = command.Content,
                ParentCommentId = command.ParentCommentId,
            };

            _commentRepository.AddComment(comment);

            // Incrémenter le compteur de commentaires de la vidéo
            video.CommentCount++;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Comment created: {CommentId} by {UserId} on video {VideoId}",
                comment.Id, userContext.LocalUserId, command.VideoId);

            return Result.Success(CreateCommentResult.Success(comment.Id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating comment");
            return Result.Failure<CreateCommentResult>(CommentErrors.ErrorCreating(ex.Message));
        }
    }
}
