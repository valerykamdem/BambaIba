using BambaIba.Domain.Entities.MediaAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BambaIba.Infrastructure.Configurations;

public class MediaAssetsConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        // Table commune MediaBase
        //builder.ToTable("Media");
        builder.HasKey(m => m.Id);

        builder.HasMany(m => m.Reactions)
        .WithOne()
        .HasForeignKey(r => r.MediaId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Property(m => m.Title).IsRequired().HasMaxLength(255);
        builder.Property(m => m.Description).HasMaxLength(2000);
        builder.Property(m => m.UserId).IsRequired();

        // Fichier
        builder.Property(m => m.ThumbnailPath).HasMaxLength(500);
        builder.Property(m => m.StoragePath).HasMaxLength(500);
        builder.Property(m => m.FileName).HasMaxLength(255);
        builder.Property(m => m.FileSize).IsRequired();

        // Métadonnées
        builder.Property(m => m.Status).IsRequired();
        builder.Property(m => m.Duration).IsRequired();
        builder.Property(m => m.IsPublic).HasDefaultValue(true);
        builder.Property(m => m.PublishedAt);

        // Tags (conversion + comparer)
        builder.Property(m => m.Tags)
              .HasConversion(
                  v => string.Join(',', v),
                  v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
              )
              .Metadata.SetValueComparer(
                  new ValueComparer<List<string>>(
                      (c1, c2) => c1.SequenceEqual(c2),
                      c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                      c => c.ToList()
                  )
              );
    }
}

