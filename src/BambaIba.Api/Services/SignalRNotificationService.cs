using BambaIba.Api.Hubs;
using BambaIba.Application.Abstractions.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace BambaIba.Api.Services;

// This class lives in the API layer because it needs IHubContext
public class SignalRNotificationService(IHubContext<NotificationHub> hubContext) : INotificationService
{
    public async Task PushNotificationAsync(Guid recipientUserId, object notificationPayload)
    {
        // Calculate the group name (must match logic in NotificationHub)
        string groupName = $"user-{recipientUserId}";

        // Send via SignalR
        await hubContext.Clients.Group(groupName)
            .SendAsync("ReceiveNotification", notificationPayload);
    }
}
