using BambaIba.Domain.Entities.MediaChannels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BambaIba.Infrastructure.Configurations;

public class MediaChannelsConfiguration : IEntityTypeConfiguration<MediaChannel>
{
    public void Configure(EntityTypeBuilder<MediaChannel> builder)
    {
        // Configure Channel Ownership
        builder.HasOne(c => c.User)
            .WithMany(u => u.MediaChannels)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique Handle for Channel (ex: @bambaiba)
        builder.HasIndex(c => c.Handle)
            .IsUnique();
    }
}
