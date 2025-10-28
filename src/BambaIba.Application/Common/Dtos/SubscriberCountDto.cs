namespace BambaIba.Application.Common.Dtos;
public record SubscriberCountDto
{
    public string ChannelId { get; init; } = string.Empty;
    public int SubscriberCount { get; init; }
}
