using BambaIba.Domain.Shared;

namespace BambaIba.Domain.Entities;
public class Comment : Entity<Guid>, ISoftDeletable
{
    public Guid VideoId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }
    //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsEdited { get; set; }
}
