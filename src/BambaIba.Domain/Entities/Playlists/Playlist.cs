using BambaIba.Domain.Entities.PlaylistItems;
using BambaIba.Domain.Entities.Users;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities.Playlists;
public sealed class Playlist : Entity<Guid>, ISoftDeletable
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid? ChannelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PlaylistVisibility Visibility { get; set; } = PlaylistVisibility.Private;
    public string? ThumbnailUrl { get; set; }
    public int MediaCount { get; set; } = 0;

    // Relations
    public ICollection<PlaylistItem> Items { get; set; } = [];
}
