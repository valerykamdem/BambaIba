namespace BambaIba.Api.Features.Comments;

public record UpdateCommentRequest
{
    public string Content { get; init; } = string.Empty;
}
