using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Comments;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.Comments.UpdateComment;
public sealed record UpdateCommentCommand(
    Guid CommentId,
    Guid UserId,
    string Content) : ICommand<Result<UpdateCommentResult>>;
