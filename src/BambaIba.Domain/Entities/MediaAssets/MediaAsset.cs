using BambaIba.Domain.Entities.MediaStats;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities.MediaAssets;

public abstract class MediaAsset : Entity<Guid>, ISoftDeletable
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Speaker { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public Guid UserId { get; set; }

    // File and Stockage
    public string ThumbnailPath { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }

    // Metadonnées
    public MediaStatus Status { get; set; }
    public TimeSpan Duration { get; set; }
    public bool IsPublic { get; set; } = true;
    public DateTime? PublishedAt { get; set; }
    public List<string> Tags { get; set; } = [];

    public MediaStat Stat { get; set; } = default!;
}
