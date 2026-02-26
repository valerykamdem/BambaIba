using System.Data;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Audios;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.MediaChannels;
using BambaIba.Domain.Entities.MediaReactions;
using BambaIba.Domain.Entities.MediaStats;
using BambaIba.Domain.Entities.PlaylistItems;
using BambaIba.Domain.Entities.Playlists;
using BambaIba.Domain.Entities.Roles;
using BambaIba.Domain.Entities.Users;
using BambaIba.Domain.Entities.UserSubscriptions;
using BambaIba.Domain.Entities.VideoQualities;
using BambaIba.Domain.Entities.Videos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BambaIba.Infrastructure.Persistence;

public sealed class BIDbContext : DbContext, IBIDbContext
{
    public BIDbContext(DbContextOptions<BIDbContext> options) : base(options)
    {
    }

    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
    public DbSet<Video> Videos => Set<Video>();
    public DbSet<MediaReaction> MediaReactions => Set<MediaReaction>();
    public DbSet<User> Users => Set<User>();
    public DbSet<MediaChannel> MediaChannels => Set<MediaChannel>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<Playlist> Playlists => Set<Playlist>();
    public DbSet<PlaylistItem> PlaylistItems => Set<PlaylistItem>();
    public DbSet<VideoQuality> VideoQualities => Set<VideoQuality>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Audio> Audios => Set<Audio>();
    public DbSet<MediaStat> MediaStats => Set<MediaStat>();

    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return (await Database.BeginTransactionAsync(cancellationToken)).GetDbTransaction();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration MediaReaction
        modelBuilder.Entity<MediaReaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne<MediaAsset>()
                .WithMany()
                .HasForeignKey(e => e.MediaId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.MediaId, e.UserId }).IsUnique();
        });

        //// Configuration View
        //modelBuilder.Entity<View>(entity =>
        //{
        //    entity.HasKey(e => e.Id);
        //    entity.HasOne<MediaAsset>()
        //        .WithMany()
        //        .HasForeignKey(e => e.VideoId)
        //        .OnDelete(DeleteBehavior.Cascade);
        //    entity.HasIndex(e => e.VideoId);
        //    entity.HasIndex(e => e.ViewedAt);
        //});       

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

        //// LiveStream
        //modelBuilder.Entity<LiveSession>(entity =>
        //{
        //    entity.HasKey(e => e.Id);
        //    entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
        //    entity.Property(e => e.RoomId).IsRequired().HasMaxLength(50);
        //    //entity.HasIndex(e => e.StreamKey).IsUnique();
        //    //entity.HasIndex(e => e.Status);
        //    //entity.HasIndex(e => e.StreamerId);
        //});

        //// LiveChatMessage
        //modelBuilder.Entity<LiveChatMessage>(entity =>
        //{
        //    entity.HasKey(e => e.Id);
        //    entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
        //    entity.HasIndex(e => e.LiveStreamId);
        //    entity.HasIndex(e => e.SentAt);
        //});


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
