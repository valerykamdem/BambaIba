
using BambaIba.Domain.LiveChatMessages;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.LiveStreams.GetChatHistory;
public sealed record GetChatHistoryQuery : IQuery<Result<GetChatHistoryResult>>
{
    public Guid StreamId { get; init; }
    public int Limit { get; init; } = 50;
}

public record GetChatHistoryResult
{
    public List<LiveChatMessage> Messages { get; init; } = [];
}

public record ChatMessageDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTime SentAt { get; init; }
    public bool IsDeleted { get; init; }
}
