using BambaIba.Domain.Entities.MediaChannels;
using BambaIba.Domain.Entities.Users;
using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities.UserSubscriptions;
public sealed class UserSubscription : Entity<Guid>, ISoftDeletable
{
    // The Viewer (User)
    public Guid FollowerId { get; set; }
    public Guid ChannelId { get; set; }
    public User Follower { get; set; }
    public MediaChannel MediaChannel { get; set; }
    public bool NotificationsEnabled { get; set; } = true;
}
