// BambaIba.Domain/Entities/Audio.cs
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;

namespace BambaIba.Domain.Audios;

public class Audio : Entity<Guid>, ISoftDeletable
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Artist { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string CoverImagePath { get; set; } = string.Empty; // Thumbnail
    public TimeSpan Duration { get; set; }
    public long FileSize { get; set; }
    public MediaStatus Status { get; set; }
    public int PlayCount { get; set; }
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }
    public int CommentCount { get; set; }
    public bool IsPublic { get; set; } = true;
    public DateTime? PublishedAt { get; set; }
    public List<string> Tags { get; set; } = [];
}

