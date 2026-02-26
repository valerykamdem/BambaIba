using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BambaIba.Api.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Récupère l'ID de l'utilisateur depuis le Token JWT
        string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? Context.User?.FindFirst("sub")?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            // On ajoute l'utilisateur à un groupe nommé avec son ID.
            // Cela permet d'envoyer des messages ciblés : "Send to User 123"
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");

            // Log pour debug
            // logger.LogInformation("User {UserId} connected to NotificationHub", userId);
        }

        await base.OnConnectedAsync();
    }
}
