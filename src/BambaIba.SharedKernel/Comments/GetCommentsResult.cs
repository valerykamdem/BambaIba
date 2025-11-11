namespace BambaIba.SharedKernel.Comments;
public sealed record GetCommentsResult
{
    public List<CommentDto> Comments { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

public sealed record CommentDto
{
    public Guid Id { get; init; }
    public Guid VideoId { get; init; }
    public Guid UserId { get; init; }
    public string Content { get; init; } = string.Empty;
    public Guid? ParentCommentId { get; init; }
    public int LikeCount { get; init; }
    public int ReplyCount { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsEdited { get; init; }
}
