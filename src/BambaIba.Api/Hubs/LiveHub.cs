// BambaIba.Api/Hubs/LiveChatHub.cs
using BambaIba.Domain.Entities.LiveChatMessages;
using BambaIba.Domain.Entities.LiveStream;
using BambaIba.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BambaIba.Api.Hubs;

// Modifier LiveChatHub pour gérer le compteur de viewers
public class LiveHub : Hub
{
    private readonly BambaIbaDbContext _context;
    private readonly ILogger<LiveHub> _logger;

    public LiveHub(
        BambaIbaDbContext context,
        ILogger<LiveHub> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Méthode ouverte à tous (radio libre)
    public async Task SubscribeToRadio()
    {
        await Clients.Caller.SendAsync("Info", "Connecté au flux radio");
    }

    public async Task JoinStream(string streamId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, streamId);

        // Incrémenter le viewer count
        if (Guid.TryParse(streamId, out Guid streamGuid))
        {
            LiveStream? stream = await _context.LiveStreams.FindAsync(streamGuid);
            if (stream != null)
            {
                stream.ViewerCount++;
                await _context.SaveChangesAsync();

                // Notifier le nouveau count
                await Clients.Group(streamId).SendAsync("ViewerCountUpdated", stream.ViewerCount);
            }
        }

        await Clients.Group(streamId).SendAsync("UserJoined", new
        {
            UserId = Context.UserIdentifier,
            UserName = Context.User?.Identity?.Name
        });
    }

    public async Task LeaveStream(string streamId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, streamId);

        // Décrémenter le viewer count
        if (Guid.TryParse(streamId, out Guid streamGuid))
        {
            LiveStream? stream = await _context.LiveStreams.FindAsync(streamGuid);
            if (stream != null && stream.ViewerCount > 0)
            {
                stream.ViewerCount--;
                await _context.SaveChangesAsync();

                await Clients.Group(streamId).SendAsync("ViewerCountUpdated", stream.ViewerCount);
            }
        }

        await Clients.Group(streamId).SendAsync("UserLeft", new
        {
            UserId = Context.UserIdentifier
        });
    }

    [Authorize]
    public async Task SendMessage(string streamId, string message)
    {
        string? userId = Context.UserIdentifier;
        string userName = Context.User?.Identity?.Name ?? "Anonymous";

        if (string.IsNullOrEmpty(userId))
            throw new HubException("User must be authenticated to send messages.");

        if (string.IsNullOrWhiteSpace(message) || message.Length > 500)
            return;

        // Sauvegarder le message en DB (optionnel pour historique)
        if (Guid.TryParse(streamId, out Guid streamGuid))
        {
            var chatMessage = new LiveChatMessage
            {
                Id = Guid.CreateVersion7(),
                LiveStreamId = streamGuid,
                UserId = Guid.Parse(userId),
                UserName = userName,
                Message = message,
                SentAt = DateTime.UtcNow
            };

            _context.LiveChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();
        }

        var messageDto = new
        {
            Id = Guid.CreateVersion7(),
            StreamId = streamId,
            UserId = userId,
            UserName = userName,
            Message = message,
            SentAt = DateTime.UtcNow
        };

        await Clients.Group(streamId).SendAsync("ReceiveMessage", messageDto);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Gérer la déconnexion automatique
        // Note: Il faudrait tracker dans quel stream l'user était
        _logger.LogInformation("User {UserId} disconnected", Context.UserIdentifier);
        await base.OnDisconnectedAsync(exception);
    }

    // Ajouter dans LiveChatHub
    [Authorize(Roles = "Moderator,Streamer")]
    public async Task DeleteMessage(string streamId, string messageId)
    {
        if (Guid.TryParse(messageId, out Guid msgGuid))
        {
            LiveChatMessage? message = await _context.LiveChatMessages.FindAsync(msgGuid);
            if (message != null)
            {
                message.IsDeleted = true;
                await _context.SaveChangesAsync();

                await Clients.Group(streamId).SendAsync("MessageDeleted", messageId);
            }
        }
    }

    [Authorize(Roles = "Moderator,Streamer")]
    public async Task BanUser(string streamId, string userId)
    {
        // Implémenter la logique de ban
        // Stocker dans Redis ou DB

        await Clients.User(userId).SendAsync("Banned", new
        {
            StreamId = streamId,
            Reason = "Vous avez été banni de ce chat"
        });

        // Déconnecter l'utilisateur du groupe
        List<string> connections = await GetUserConnections(userId);
        foreach (string connectionId in connections)
        {
            await Groups.RemoveFromGroupAsync(connectionId, streamId);
        }
    }

    private async Task<List<string>> GetUserConnections(string userId)
    {
        // Implémenter tracking des connexions (Redis)
        string ui = userId;
        return [];
    }
}
