using System.ComponentModel.DataAnnotations;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.Users;
using BambaIba.Domain.Entities.UserSubscriptions;
using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities.MediaChannels;

public sealed class MediaChannel : Entity<Guid>, ISoftDeletable
{

    // The owner of the channel
    public Guid UserId { get; set; }
    public User User { get; set; }

    // Channel Details
    [MaxLength(60)]
    public string Name { get; set; } = string.Empty; // "Tech Daily"

    [MaxLength(500)]
    public string? Description { get; set; }

    public string? Handle { get; set; } // @techdaily (Unique handle)

    public string? AvatarUrl { get; set; }
    public string? BannerUrl { get; set; }

    // --- Relations ---
    // Videos belong to a Channel, not directly to a User
    public ICollection<MediaAsset> MediaAssets { get; set; } = [];

    // Subscribers belong to a Channel
    public ICollection<UserSubscription> Subscribers { get; set; } = [];
}
