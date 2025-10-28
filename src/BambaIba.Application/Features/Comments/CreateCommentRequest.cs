namespace BambaIba.Api.Features.Comments;

public record CreateCommentRequest
{
    public string Content { get; init; } = string.Empty;
    public Guid? ParentCommentId { get; init; }
}
