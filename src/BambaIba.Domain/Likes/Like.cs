using BambaIba.SharedKernel;

namespace BambaIba.Domain.Likes;
public sealed class Like : Entity<Guid>, ISoftDeletable
{
    public Guid MediaId { get; set; }
    public Guid UserId { get; set; }
    public bool IsLiked { get; set; } // true = like, false = dislike
}
