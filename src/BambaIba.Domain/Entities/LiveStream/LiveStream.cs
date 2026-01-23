using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities.LiveStream;
public sealed class LiveStream : Entity<Guid>, ISoftDeletable
{
    //public Guid Id { get; set; }
    public string StreamerId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string StreamKey { get; set; } = string.Empty; // Pour RTMP
    public string ThumbnailPath { get; set; } = string.Empty;
    public LiveStreamStatus Status { get; set; }
    public int ViewerCount { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublic { get; set; } = true;
}
