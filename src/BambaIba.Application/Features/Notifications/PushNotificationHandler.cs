using BambaIba.Application.Abstractions.DomainEvents;
using BambaIba.Application.Abstractions.Interfaces;

namespace BambaIba.Application.Features.Notifications;

public class PushNotificationHandler(INotificationService notificationService)
{
    // Wolverine écoute l'événement ici
    public async Task Handle(NotificationCreatedEvent notification)
    {
        // 1. Prepare the DTO for the Frontend Client
        var clientNotification = new
        {
            Type = notification.MessageType,
            Message = notification.MessageContent,
            User = notification.TriggeredByUsername,
            notification.MediaId,
            notification.MediaTitle,
            Timestamp = DateTime.UtcNow
        };

        // 2. Use the abstracted service (No dependency on SignalR here!)
        await notificationService.PushNotificationAsync(
            notification.RecipientUserId,
            clientNotification);
    }
}
