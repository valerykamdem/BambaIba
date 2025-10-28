namespace BambaIba.Application.Common.Dtos;
public record SubscriptionDto
{
    public string ChannelId { get; init; } = string.Empty;
    public bool IsSubscribed { get; init; }
    public DateTime? SubscribedAt { get; init; }
    public bool NotificationsEnabled { get; init; }
}
