using BambaIba.Domain.Entities.MediaBase;
using BambaIba.Domain.Entities.Playlists;
using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities.PlaylistItems;
public sealed class PlaylistItem : Entity<Guid>, ISoftDeletable
{
    public Guid PlaylistId { get; set; }
    public Playlist Playlist { get; set; } = null!;
    public Guid MediaId { get; set; }
    public Media Media { get; set; } = null!;
    public int Position { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
