
using BambaIba.Domain.Shared;

namespace BambaIba.Domain.Entities;
public class VideoQuality : Entity<Guid>, ISoftDeletable
{
    public Guid VideoId { get; set; }
    public string Quality { get; set; } = string.Empty; //240p, 360p, 480p, 720p, 1080p
    public string StoragePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
