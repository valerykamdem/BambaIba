using BambaIba.SharedKernel;

namespace BambaIba.Domain.Likes;
public sealed class Like : Entity<Guid>, ISoftDeletable
{
    public Guid VideoId { get; set; }
    public Guid UserId { get; set; }
    public bool IsLike { get; set; } // true = like, false = dislike
}
