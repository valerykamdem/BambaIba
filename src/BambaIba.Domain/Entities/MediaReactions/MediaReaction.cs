using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities.MediaReactions;
public sealed class MediaReaction : Entity<Guid>, ISoftDeletable
{
    public Guid MediaId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType ReactionType { get; set; }
}
