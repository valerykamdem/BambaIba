using System.Text.Json;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Mongo.LiveChatMessages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace BambaIba.Application.Features.LiveChats;

public record ArchiveLiveChatCommand(Guid LiveEventId);

public class ArchiveLiveChatHandler(
    IConnectionMultiplexer redis,
    IBIMongoContext mongoContext,
    ILogger<ArchiveLiveChatHandler> logger)
{
    public async Task Handle(ArchiveLiveChatCommand command)
    {
        IDatabase db = redis.GetDatabase();
        string redisKey = $"chat:{command.LiveEventId}";

        logger.LogInformation("Starting chat archival for Live: {LiveId}", command.LiveEventId);

        // 1. Fetch all messages from Redis
        RedisValue[] messages = await db.ListRangeAsync(redisKey);

        if (messages.Length == 0)
        {
            logger.LogInformation("No chat messages found to archive.");
            return;
        }

        // 2. Convert to Domain Entities
        var chatLogs = new List<LiveChatMessage>();

        foreach (RedisValue msg in messages)
        {
            try
            {
                string jsonString = msg.ToString();

                if (string.IsNullOrEmpty(jsonString))
                    continue;

                // Deserialize back from Redis JSON
                LiveMessageDto dto = JsonSerializer.Deserialize<LiveMessageDto>(jsonString);

                if (dto != null)
                {
                    chatLogs.Add(new LiveChatMessage
                    {
                        Id = Guid.CreateVersion7(),
                        LiveEventId = command.LiveEventId,
                        UserId = dto.UserId,
                        Username = dto.Username,
                        Content = dto.Content,
                        SentAt = dto.SentAt
                    });
                }
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Failed to parse chat message during archival.");
            }
        }

        // 3. Bulk Insert into MongoDB (Cold Storage)
        if (chatLogs.Count != 0)
        {
            await mongoContext.LiveChatMessages.InsertManyAsync(chatLogs);
            logger.LogInformation("Archived {Count} messages to MongoDB.", chatLogs.Count);
        }

        // 4. Cleanup Redis: Delete the list to free memory
        await db.KeyDeleteAsync(redisKey);
        logger.LogInformation("Cleaned up Redis cache for Live: {LiveId}", command.LiveEventId);
    }
}
