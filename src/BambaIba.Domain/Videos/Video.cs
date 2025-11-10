using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Enums;

namespace BambaIba.Domain.Videos;
public sealed class Video : Entity<Guid>, ISoftDeletable
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public long FileSize { get; set; }
    public VideoStatus Status { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }
    public int CommentCount { get; set; }
    public bool IsPublic { get; set; } = true;
    public DateTime? PublishedAt { get; set; }
    public List<string> Tags { get; set; } = [];
}
