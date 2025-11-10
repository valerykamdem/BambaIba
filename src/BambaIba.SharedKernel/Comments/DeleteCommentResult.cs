
namespace BambaIba.SharedKernel.Comments;
public sealed record DeleteCommentResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }

    public static DeleteCommentResult Success() => new() { IsSuccess = true };
    public static DeleteCommentResult Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}
