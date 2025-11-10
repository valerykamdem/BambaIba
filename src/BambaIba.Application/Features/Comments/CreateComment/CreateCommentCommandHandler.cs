using BambaIba.Application.Abstractions.Data;
using BambaIba.Domain.Comments;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Comments.CreateComment;
public sealed class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand, Result<CreateCommentResult>>
{
    private readonly ICommentRepository _commentRepository; 
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCommentCommandHandler> _logger;

    public CreateCommentCommandHandler(
        ICommentRepository commentRepository,
        IVideoRepository videoRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateCommentCommandHandler> logger)
    {
        _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
        _videoRepository = videoRepository ?? throw new ArgumentNullException(nameof(videoRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<CreateCommentResult>> Handle(CreateCommentCommand command, CancellationToken cancellationToken)
    {
        try
        {
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
                UserId = command.UserId,
                Content = command.Content,
                ParentCommentId = command.ParentCommentId,
            };

            _commentRepository.AddComment(comment);

            // Incrémenter le compteur de commentaires de la vidéo
            video.CommentCount++;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Comment created: {CommentId} by {UserId} on video {VideoId}",
                comment.Id, command.UserId, command.VideoId);

            return Result.Success(CreateCommentResult.Success(comment.Id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating comment");
            return Result.Failure<CreateCommentResult>(CommentErrors.ErrorCreating(ex.Message));
        }
    }
}
