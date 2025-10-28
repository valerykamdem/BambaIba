using BambaIba.Domain.Shared;

namespace BambaIba.Domain.Entities;
public class Like : Entity<Guid>, ISoftDeletable
{
    public Guid VideoId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public bool IsLike { get; set; } // true = like, false = dislike
    //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
