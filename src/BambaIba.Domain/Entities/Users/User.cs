using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.MediaChannels;
using BambaIba.Domain.Entities.MediaReactions;
using BambaIba.Domain.Entities.UserSubscriptions;
using BambaIba.SharedKernel;


namespace BambaIba.Domain.Entities.Users;

public sealed class User : Entity<Guid>, ISoftDeletable
{
    public string IdentityId { get; set; } = string.Empty;
    public string CivilStatus { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Pseudo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // --- Channel Info (Les infos de la chaîne) ---
    public string ChannelName { get; set; } = string.Empty;
    public string? ChannelDescription { get; set; }
    public string? ChannelBannerUrl { get; set; }
    public string? ChannelAvatarUrl { get; set; }


    public ICollection<UserRole> UserRoles { get; set; } = [];
    // --- Relations ---
    public ICollection<MediaChannel> MediaChannels { get; set; } = [];
    // Les gens qu'il suit
    public ICollection<UserSubscription> UserSubscriptions { get; set; } = [];
}
