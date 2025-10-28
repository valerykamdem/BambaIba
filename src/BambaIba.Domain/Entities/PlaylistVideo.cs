
using BambaIba.Domain.Shared;

namespace BambaIba.Domain.Entities;
public class PlaylistVideo : Entity<Guid>, ISoftDeletable
{
    public Guid PlaylistId { get; set; }
    public Playlist Playlist { get; set; } = null!;
    public Guid VideoId { get; set; }
    public Video Video { get; set; } = null!;
    public int Position { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
