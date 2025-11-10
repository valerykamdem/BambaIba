namespace BambaIba.Application.Abstractions.Dtos;
public record CommentDto
{
    public Guid Id { get; init; }
    public Guid VideoId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public Guid? ParentCommentId { get; init; }
    public int LikeCount { get; init; }
    public int DislikeCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsEdited { get; init; }
    public int ReplyCount { get; init; }
}
