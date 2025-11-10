using BambaIba.Domain.Comments;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Comments;
using Cortex.Mediator.Commands;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Comments.UpdateComment;
public sealed class UpdateCommentCommandHandler : ICommandHandler<UpdateCommentCommand, Result<UpdateCommentResult>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<UpdateCommentCommandHandler> _logger;

    public UpdateCommentCommandHandler(
        ICommentRepository commentRepository,
        ILogger<UpdateCommentCommandHandler> logger)
    {
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateCommentResult>> Handle(UpdateCommentCommand command, CancellationToken cancellationToken)
    {
        try
        {
            Comment comment = await _commentRepository.GetComment(command.CommentId);

            if (comment == null)
                return UpdateCommentResult.Failure("Comment not found");

            // Vérifier que c'est le propriétaire
            if (comment.UserId != command.UserId)
                return UpdateCommentResult.Failure("Unauthorized");

            comment.Content = command.Content;
            comment.UpdatedAt = DateTime.UtcNow;
            comment.IsEdited = true;

            await _commentRepository.UpdateComment(comment, cancellationToken);

            _logger.LogInformation("Comment updated: {CommentId}", comment.Id);

            return Result.Success(UpdateCommentResult.Success(comment.Id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment");
            return UpdateCommentResult.Failure("An error occurred");
        }
    }
}
