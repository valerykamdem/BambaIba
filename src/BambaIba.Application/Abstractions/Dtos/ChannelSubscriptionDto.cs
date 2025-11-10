
namespace BambaIba.Application.Abstractions.Dtos;
public record ChannelSubscriptionDto
{
    public string ChannelId { get; init; } = string.Empty;
    public DateTime SubscribedAt { get; init; }
    public bool NotificationsEnabled { get; init; }
    public int VideoCount { get; init; }
}
