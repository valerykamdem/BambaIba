using System.Data;
using BambaIba.Application.Abstractions.Data;
using BambaIba.Domain.Audios;
using BambaIba.Domain.Comments;
using BambaIba.Domain.Entities;
using BambaIba.Domain.Likes;
using BambaIba.Domain.LiveChatMessages;
using BambaIba.Domain.LiveStream;
using BambaIba.Domain.MediaBase;
using BambaIba.Domain.PlaylistItems;
using BambaIba.Domain.Playlists;
using BambaIba.Domain.Users;
using BambaIba.Domain.VideoQualities;
using BambaIba.Domain.Videos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace BambaIba.Infrastructure.Persistence;

public sealed class BambaIbaDbContext : DbContext, IUnitOfWork
{
    public BambaIbaDbContext(DbContextOptions<BambaIbaDbContext> options) : base(options)
    {
    }

    public DbSet<Media> Media { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<View> Views { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<PlaylistItem> PlaylistItems { get; set; }
    public DbSet<VideoQuality> VideoQualities { get; set; }
    public DbSet<TranscodeJob> TranscodeJobs { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<LiveStream> LiveStreams { get; set; }
    public DbSet<LiveChatMessage> LiveChatMessages { get; set; }
    public DbSet<Audio> Audios { get; set; }

    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return (await Database.BeginTransactionAsync(cancellationToken)).GetDbTransaction();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration Comment
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
            entity.HasOne<Media>()
                .WithMany()
                .HasForeignKey(e => e.MediaId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.MediaId);
            entity.HasIndex(e => e.ParentCommentId);
        });

        // Configuration Like
        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<Media>()
                .WithMany()
                .HasForeignKey(e => e.MediaId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.MediaId, e.UserId }).IsUnique();
        });

        // Configuration View
        modelBuilder.Entity<View>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<Media>()
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
        modelBuilder.Entity<PlaylistItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Playlist)
                .WithMany(p => p.Items)
                .HasForeignKey(e => e.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Media)
                .WithMany()
                .HasForeignKey(e => e.MediaId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.PlaylistId, e.MediaId }).IsUnique();
        });

        // Configuration VideoQuality
        modelBuilder.Entity<TranscodeJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<Media>()
                .WithMany()
                .HasForeignKey(e => e.VideoId)
                .OnDelete(DeleteBehavior.Cascade);
            //entity.HasIndex(e => new { e.VideoId, e.Quality }).IsUnique();
        });

        // Configurer la relation User - Role
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        // LiveStream
        modelBuilder.Entity<LiveStream>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.StreamKey).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.StreamKey).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.StreamerId);
        });

        // LiveChatMessage
        modelBuilder.Entity<LiveChatMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.LiveStreamId);
            entity.HasIndex(e => e.SentAt);
        });

        // Table commune MediaBase
        modelBuilder.Entity<Media>(entity =>
        {
            //entity.ToTable("Media");
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Title).IsRequired().HasMaxLength(255);
            entity.Property(m => m.Description).HasMaxLength(2000);
            entity.Property(m => m.UserId).IsRequired();

            // Fichier
            entity.Property(m => m.ThumbnailPath).HasMaxLength(500);
            entity.Property(m => m.StoragePath).HasMaxLength(500);
            entity.Property(m => m.FileName).HasMaxLength(255);
            entity.Property(m => m.FileSize).IsRequired();

            // Métadonnées
            entity.Property(m => m.Status).IsRequired();
            entity.Property(m => m.Duration).IsRequired();
            entity.Property(m => m.LikeCount);
            entity.Property(m => m.DislikeCount);
            entity.Property(m => m.PlayCount);
            entity.Property(m => m.CommentCount);
            entity.Property(m => m.IsPublic).HasDefaultValue(true);
            entity.Property(m => m.PublishedAt);

            // Tags (conversion + comparer)
            entity.Property(m => m.Tags)
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
        });

        // Configuration Video (hérite de MediaBase)
        modelBuilder.Entity<Video>(entity =>
        {
            //entity.ToTable("Video");

            // Relation avec VideoQuality
            entity.HasMany(v => v.Qualities)
                  .WithOne()
                  .HasForeignKey(q => q.MediaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration Audio (hérite de MediaBase)
        modelBuilder.Entity<Audio>(entity =>
        {
            //entity.ToTable("Audio");

            entity.Property(a => a.Speaker).HasMaxLength(255);
            entity.Property(a => a.Category).HasMaxLength(100);
            entity.Property(a => a.Topic).HasMaxLength(255);
        });

        // Configuration VideoQuality
        modelBuilder.Entity<VideoQuality>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<Video>()
                .WithMany(v => v.Qualities)
                .HasForeignKey(e => e.MediaId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.MediaId, e.Quality }).IsUnique();
        });
    }
}
