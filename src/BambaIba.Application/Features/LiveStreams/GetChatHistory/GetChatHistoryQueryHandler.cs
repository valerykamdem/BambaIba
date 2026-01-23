// GetChatHistoryQueryHandler.cs
using BambaIba.Domain.Entities.LiveChatMessages;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.LiveStreams.GetChatHistory;

public class GetChatHistoryQueryHandler : IQueryHandler<GetChatHistoryQuery, Result<GetChatHistoryResult>>
{
    private readonly IChatMessageRepository _chatMessageRepository;

    public GetChatHistoryQueryHandler(
        IChatMessageRepository chatMessageRepository)
    {
        _chatMessageRepository = chatMessageRepository;
    }

    public async Task<Result<GetChatHistoryResult>> Handle(
        GetChatHistoryQuery request,
        CancellationToken cancellationToken)
    {
        //UserContext userContext = await _userContextService
        //        .GetCurrentContext(_httpContextAccessor.HttpContext);

        List<LiveChatMessage> messages = await _chatMessageRepository
            .GetChatHistoryMessages(request.StreamId, request.Limit, cancellationToken);

        //messages.Select(m => new ChatMessageDto
        //    {
        //        Id = m.Id,
        //        UserId = m.UserId,
        //        UserName = m.UserName,
        //        Message = m.Message,
        //        SentAt = m.SentAt,
        //        IsDeleted = m.IsDeleted
        //    })
        //    .ToListAsync(cancellationToken);

        messages.Reverse(); // Ordre chronologique

        return Result.Success(new GetChatHistoryResult { Messages = messages });
    }
}
