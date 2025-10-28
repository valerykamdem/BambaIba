using BambaIba.Domain.Shared;

namespace BambaIba.Domain.Entities;
public class View : Entity<Guid>, ISoftDeletable
{
    public Guid VideoId { get; set; }
    public string? UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan WatchDuration { get; set; }
}
