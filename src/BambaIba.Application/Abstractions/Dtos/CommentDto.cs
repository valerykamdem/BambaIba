namespace BambaIba.Application.Abstractions.Dtos;
public sealed record CommentDto
{
    public Guid Id { get; init; }
    public Guid MediaId { get; init; }
    public Guid UserId { get; init; }
    public string Content { get; init; } = string.Empty;
    public Guid? ParentCommentId { get; init; }
    public int LikeCount { get; init; }
    public int ReplayCount { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsEdited { get; init; }
}
