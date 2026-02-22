using BambaIba.Domain.Entities.MediaStats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BambaIba.Infrastructure.Configurations;

public class MediaStatsConfiguration : IEntityTypeConfiguration<MediaStat>
{
    public void Configure(EntityTypeBuilder<MediaStat> builder)
    {
        //builder.ToTable("MediaStats");

        // Primary Key
        builder.HasKey(ms => ms.MediaId);

        // One-to-one with MediaAsset
        builder.HasOne(ms => ms.Media)
            .WithOne(m => m.Stat) // ou .WithOne(m => m.Stats) si tu ajoutes une prop Stats dans MediaAsset
            .HasForeignKey<MediaStat>(ms => ms.MediaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Default values
        builder.Property(ms => ms.PlayCount).HasDefaultValue(0);
        builder.Property(ms => ms.LikeCount).HasDefaultValue(0);
        builder.Property(ms => ms.DislikeCount).HasDefaultValue(0);
        builder.Property(ms => ms.CommentCount).HasDefaultValue(0);

        // Indexes (important pour les workers Wolverine)
        builder.HasIndex(ms => ms.PlayCount);
        builder.HasIndex(ms => ms.LikeCount);
        builder.HasIndex(ms => ms.CommentCount);
    }
}
