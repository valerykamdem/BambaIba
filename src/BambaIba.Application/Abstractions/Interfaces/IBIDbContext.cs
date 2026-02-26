using System.Data;
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

namespace BambaIba.Application.Abstractions.Interfaces;

public interface IBIDbContext
{
    DbSet<MediaAsset> MediaAssets { get; }
    DbSet<Video> Videos { get; }
    DbSet<MediaReaction> MediaReactions { get; }
    DbSet<User> Users { get; }
    DbSet<MediaChannel> MediaChannels { get; }
    DbSet<UserSubscription> UserSubscriptions { get; }
    DbSet<Playlist> Playlists { get; }
    DbSet<PlaylistItem> PlaylistItems { get; }
    DbSet<VideoQuality> VideoQualities { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<Audio> Audios { get; }
    DbSet<MediaStat> MediaStats { get; }

    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DbSet<T> Set<T>() where T : class;
}
