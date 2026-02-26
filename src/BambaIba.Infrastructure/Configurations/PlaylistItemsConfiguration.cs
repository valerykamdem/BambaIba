using BambaIba.Domain.Entities.PlaylistItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BambaIba.Infrastructure.Configurations;

public class PlaylistItemsConfiguration : IEntityTypeConfiguration<PlaylistItem>
{
    public void Configure(EntityTypeBuilder<PlaylistItem> builder)
    {
        // Configuration PlaylistVideo
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Playlist)
                .WithMany(p => p.Items)
                .HasForeignKey(e => e.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Media)
                .WithMany()
                .HasForeignKey(e => e.MediaId)
                .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.PlaylistId, e.MediaId }).IsUnique();
    }
}
