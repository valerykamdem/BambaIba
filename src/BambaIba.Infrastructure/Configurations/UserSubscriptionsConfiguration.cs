using BambaIba.Domain.Entities.UserSubscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BambaIba.Infrastructure.Configurations;

public class UserSubscriptionsConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        // Configuration Subscription
        builder.HasOne(s => s.Follower)
             .WithMany(u => u.UserSubscriptions)
             .HasForeignKey(s => s.FollowerId)
             .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.MediaChannel)
            .WithMany(c => c.Subscribers)
            .HasForeignKey(s => s.ChannelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.FollowerId, s.ChannelId })
            .IsUnique();
    }
}
