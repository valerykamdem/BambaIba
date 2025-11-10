namespace BambaIba.SharedKernel.Comments;
public sealed record UpdateCommentResult
{
    public bool IsSuccess { get; init; }
    public Guid CommentId { get; init; }
    public string? ErrorMessage { get; init; }

    public static UpdateCommentResult Success(Guid commentId) => new() 
    { 
        IsSuccess = true,
        CommentId = commentId
    };
    public static UpdateCommentResult Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}
