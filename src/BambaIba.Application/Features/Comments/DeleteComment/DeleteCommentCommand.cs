using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Comments;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.Comments.DeleteComment;
public sealed record DeleteCommentCommand(Guid CommentId, Guid VideoId) : 
    ICommand<Result<DeleteCommentResult>>;
