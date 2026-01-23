using System.Threading;
using BambaIba.Application.Features.LiveStreams.GetChatHistory;
using BambaIba.Domain.Entities.LiveChatMessages;
using BambaIba.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BambaIba.Infrastructure.Repositories;
public class ChatMessageRepository : IChatMessageRepository
{
    private readonly BambaIbaDbContext _dbContext;
    public ChatMessageRepository(BambaIbaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<LiveChatMessage>> GetChatHistoryMessages(
        Guid streamId, int limit, CancellationToken cancellationToken)
    {
        List<LiveChatMessage> messages = await _dbContext.LiveChatMessages
            .Where(m => m.LiveStreamId == streamId)
            .OrderByDescending(m => m.SentAt)
            .Take(limit).ToListAsync(cancellationToken);
        messages.Reverse();
        return messages;
    }
}
