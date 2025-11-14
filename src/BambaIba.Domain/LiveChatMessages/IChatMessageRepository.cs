
namespace BambaIba.Domain.LiveChatMessages;
public interface IChatMessageRepository
{
    Task<List<LiveChatMessage>> GetChatHistoryMessages(
        Guid streamId, 
        int limit,
        CancellationToken cancellationToken);
    //Task<LiveChatMessage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    //Task AddAsync(LiveChatMessage chatMessage);
    //Task UpdateAsync(LiveChatMessage chatMessage);
    //Task DeleteAsync(LiveChatMessage chatMessage);
}
