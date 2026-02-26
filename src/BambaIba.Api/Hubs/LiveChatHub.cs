// BambaIba.Api/Hubs/LiveChatHub.cs
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System.Text.Json;
using Wolverine;

namespace BambaIba.ApI.Hubs;

[Authorize]
public class LiveChatHub : Hub
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IUserContextService _userContext;
    private readonly ILogger<LiveChatHub> _logger;

    public LiveChatHub(
        IConnectionMultiplexer redis,
        IUserContextService userContext,
        ILogger<LiveChatHub> logger)
    {
        _redis = redis;
        _userContext = userContext;
        _logger = logger;
    }

    // 1. Client joins a specific Live Event
    public async Task JoinLiveEvent(Guid liveEventId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, liveEventId.ToString());
        _logger.LogInformation("User joined Live Event: {LiveEventId}", liveEventId);

        // Optional: Notify group that someone joined
        // await Clients.Group(liveEventId.ToString()).SendAsync("UserJoined", ...);
    }

    // 2. Client sends a message
    public async Task SendMessage(Guid liveEventId, string content)
    {
        // A. Security: Validate user
        UserContext userContext = await _userContext.GetCurrentContext() ?? throw new HubException("Unauthorized");

        // B. Validation: Basic content check
        if (string.IsNullOrWhiteSpace(content) || content.Length > 500)
            return; // Or throw

        // C. Create the Message Object
        var messageDto = new LiveMessageDto(
            userContext.LocalUserId,
            userContext.Username ?? "Anonymous",
            content,
            DateTime.UtcNow
        );

        // D. BROADCAST: Send immediately to all viewers (Real-time)
        await Clients.Group(liveEventId.ToString())
            .SendAsync("ReceiveMessage", messageDto);

        // E. PERSIST (Hot): Push to Redis List for temporary storage
        try
        {
            IDatabase db = _redis.GetDatabase();
            string jsonMessage = JsonSerializer.Serialize(messageDto);

            // Key format: "chat:{LiveEventId}"
            await db.ListRightPushAsync($"chat:{liveEventId}", jsonMessage);
        }
        catch (Exception ex)
        {
            // Log error but don't crash the user experience
            _logger.LogError(ex, "Failed to persist chat message to Redis for Live {LiveEventId}", liveEventId);
        }
    }

    // Optional: Client leaves
    public async Task LeaveLiveEvent(Guid liveEventId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, liveEventId.ToString());
    }
}
