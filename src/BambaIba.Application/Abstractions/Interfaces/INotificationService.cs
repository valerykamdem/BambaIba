
namespace BambaIba.Application.Abstractions.Interfaces;

public interface INotificationService
{
    /// <summary>
    /// Sends a real-time notification to a specific user.
    /// </summary>
    Task PushNotificationAsync(Guid recipientUserId, object notificationPayload);
}
