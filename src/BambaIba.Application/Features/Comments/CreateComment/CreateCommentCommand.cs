using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.Comments.CreateComment;
public sealed record CreateCommentCommand : ICommand<Result<CreateCommentResult>>
{
    public Guid MediaId { get; init; }
    //public Guid UserId { get; init; }
    public string Content { get; init; } = string.Empty;
    public Guid? ParentCommentId { get; init; }
}


public sealed record CreateCommentResult
{
    public bool IsSuccess { get; init; }
    public Guid CommentId { get; init; }
    public string? ErrorMessage { get; init; }

    public static CreateCommentResult Success(Guid commentId)
        => new() { IsSuccess = true, CommentId = commentId };

    public static CreateCommentResult Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}
