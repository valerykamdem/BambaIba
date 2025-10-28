
using BambaIba.Domain.Shared;

namespace BambaIba.Domain.Entities;
public class Playlist : Entity<Guid>, ISoftDeletable
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = true;
    //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<PlaylistVideo> Videos { get; set; } = new();
}
