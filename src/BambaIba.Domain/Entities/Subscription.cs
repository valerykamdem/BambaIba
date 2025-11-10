using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities;
public sealed class Subscription : Entity<Guid>, ISoftDeletable
{
    public string SubscriberId { get; set; } = string.Empty;
    public string ChannelId { get; set; } = string.Empty;
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
    public bool NotificationsEnabled { get; set; } = true;
}
