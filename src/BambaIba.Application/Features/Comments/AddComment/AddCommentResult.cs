
namespace BambaIba.Application.Features.Comments.AddComment;

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
