using BambaIba.Domain.Entities;
using BambaIba.Domain.Entities.Audios;
//using BambaIba.Domain.Entities.Comments;
using BambaIba.Domain.Entities.Likes;
using BambaIba.Domain.Entities.LiveChatMessages;
using BambaIba.Domain.Entities.LiveStream;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.MediaStats;
using BambaIba.Domain.Entities.PlaylistItems;
using BambaIba.Domain.Entities.Playlists;
using BambaIba.Domain.Entities.Users;
using BambaIba.Domain.Entities.VideoQualities;
using BambaIba.Domain.Entities.Videos;
using Microsoft.EntityFrameworkCore;

namespace BambaIba.Application.Abstractions.Interfaces;

public interface IBIDbContext
{
    DbSet<MediaAsset> MediaAssets { get; }
    DbSet<Video> Videos { get; }
    //DbSet<Comment> Comments { get; }
    DbSet<Like> Likes { get; }
    DbSet<User> Users { get; }
    DbSet<View> Views { get; }
    DbSet<Subscription> Subscriptions { get; }
    DbSet<Playlist> Playlists { get; }
    DbSet<PlaylistItem> PlaylistItems { get; }
    DbSet<VideoQuality> VideoQualities { get; }
    DbSet<TranscodeJob> TranscodeJobs { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<LiveStream> LiveStreams { get; }
    DbSet<LiveChatMessage> LiveChatMessages { get; }
    DbSet<Audio> Audios { get; }
    DbSet<MediaStat> MediaStats { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DbSet<T> Set<T>() where T : class;
}
