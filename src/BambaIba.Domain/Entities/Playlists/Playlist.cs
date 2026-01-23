using BambaIba.Domain.Entities.PlaylistItems;
using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities.Playlists;
public sealed class Playlist : Entity<Guid>, ISoftDeletable
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = true;
    public List<PlaylistItem> Items { get; set; } = [];
}
