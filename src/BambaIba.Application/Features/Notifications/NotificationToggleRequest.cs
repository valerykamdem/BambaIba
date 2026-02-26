namespace BambaIba.Application.Features.Notifications;

public record NotificationToggleRequest
{
    public bool Enabled { get; init; }
}
