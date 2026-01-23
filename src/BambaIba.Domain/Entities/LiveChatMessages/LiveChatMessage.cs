using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities.LiveChatMessages;
public sealed class LiveChatMessage: Entity<Guid>, ISoftDeletable
{
    public Guid LiveStreamId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
