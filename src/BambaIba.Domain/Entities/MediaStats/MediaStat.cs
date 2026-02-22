using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities.MediaStats;

public sealed class MediaStat : Entity<Guid>, ISoftDeletable
{
    public Guid MediaId { get; set; }
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }
    public int PlayCount { get; set; }
    public int CommentCount { get; set; }

    public MediaAsset Media { get; set; } = default!;
}
