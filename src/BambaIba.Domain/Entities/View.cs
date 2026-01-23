using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities;
public sealed class View : Entity<Guid>, ISoftDeletable
{
    public Guid VideoId { get; set; }
    public Guid? UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan WatchDuration { get; set; }
}
