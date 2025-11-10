using BambaIba.SharedKernel;

namespace BambaIba.Domain.Comments;
public sealed class Comment : Entity<Guid>, ISoftDeletable
{
    public Guid VideoId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }
    public bool IsEdited { get; set; }
}
