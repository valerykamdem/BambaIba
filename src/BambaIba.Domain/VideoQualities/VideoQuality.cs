using BambaIba.SharedKernel;

namespace BambaIba.Domain.VideoQualities;
public sealed class VideoQuality : Entity<Guid>, ISoftDeletable
{
    public Guid VideoId { get; set; }
    public string Quality { get; set; } = string.Empty; //240p, 360p, 480p, 720p, 1080p
    public string StoragePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
}
