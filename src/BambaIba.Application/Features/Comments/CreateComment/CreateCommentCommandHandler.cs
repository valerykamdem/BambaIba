using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Comments;
using BambaIba.Domain.MediaBase;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Comments.CreateComment;
public sealed class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand, Result<CreateCommentResult>>
{
    private readonly ICommentRepository _commentRepository; 
    private readonly IMediaRepository _mediaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CreateCommentCommandHandler> _logger;

    public CreateCommentCommandHandler(
        ICommentRepository commentRepository,
        IMediaRepository mediaRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<CreateCommentCommandHandler> logger)
    {
        _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
        _mediaRepository = mediaRepository ?? throw new ArgumentNullException(nameof(mediaRepository));
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

            Media media = await _mediaRepository.GetMediaByIdAsync(command.MediaId, cancellationToken);

            if (media == null)
                return Result.Failure<CreateCommentResult>(VideoErrors.NotFound(command.MediaId));

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
                MediaId = command.MediaId,
                UserId = userContext.LocalUserId,
                Content = command.Content,
                ParentCommentId = command.ParentCommentId,
            };

            await _commentRepository.AddCommentAsync(comment);

            // Incrémenter le compteur de commentaires de la vidéo
            media.CommentCount++;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Comment created: {CommentId} by {UserId} on video {MediaId}",
                comment.Id, userContext.LocalUserId, command.MediaId);

            return Result.Success(CreateCommentResult.Success(comment.Id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating comment");
            return Result.Failure<CreateCommentResult>(CommentErrors.ErrorCreating(ex.Message));
        }
    }
}
