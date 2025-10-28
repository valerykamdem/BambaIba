using BambaIba.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BambaIba.Infrastructure.Persistence;

public class BambaIbaDbContext : DbContext
{
    public BambaIbaDbContext(DbContextOptions<BambaIbaDbContext> options) : base(options)
    {
    }

    public DbSet<Video> Videos { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<View> Views { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<PlaylistVideo> PlaylistVideos { get; set; }
    public DbSet<VideoQuality> VideoQualities { get; set; }
    public DbSet<TranscodeJob> TranscodeJobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration Video
        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(5000);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Duration).IsRequired();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Status);
        });

        // Configuration Comment
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
            entity.HasOne<Video>()
                .WithMany()
                .HasForeignKey(e => e.VideoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.VideoId);
            entity.HasIndex(e => e.ParentCommentId);
        });

        // Configuration Like
        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<Video>()
                .WithMany()
                .HasForeignKey(e => e.VideoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.VideoId, e.UserId }).IsUnique();
        });

        // Configuration View
        modelBuilder.Entity<View>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<Video>()
                .WithMany()
                .HasForeignKey(e => e.VideoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.VideoId);
            entity.HasIndex(e => e.ViewedAt);
        });

        // Configuration Subscription
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SubscriberId, e.ChannelId }).IsUnique();
        });

        // Configuration Playlist
        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.UserId);
        });

        // Configuration PlaylistVideo
        modelBuilder.Entity<PlaylistVideo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Playlist)
                .WithMany(p => p.Videos)
                .HasForeignKey(e => e.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Video)
                .WithMany()
                .HasForeignKey(e => e.VideoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.PlaylistId, e.VideoId }).IsUnique();
        });

        // Configuration VideoQuality
        modelBuilder.Entity<VideoQuality>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<Video>()
                .WithMany()
                .HasForeignKey(e => e.VideoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.VideoId, e.Quality }).IsUnique();
        });

        // Configuration VideoQuality
        modelBuilder.Entity<TranscodeJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<Video>()
                .WithMany()
                .HasForeignKey(e => e.VideoId)
                .OnDelete(DeleteBehavior.Cascade);
            //entity.HasIndex(e => new { e.VideoId, e.Quality }).IsUnique();
        });
    }
}
