using BambaIba.Domain.Comments;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Comments;
using Cortex.Mediator.Queries;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Comments.GetReplies;
public sealed class GetRepliesQueryHandler : IQueryHandler<GetRepliesQuery, Result<GetRepliesResult>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<GetRepliesQueryHandler> _logger;

    public GetRepliesQueryHandler(
        ICommentRepository commentRepository,
        ILogger<GetRepliesQueryHandler> logger)
    {
        _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<GetRepliesResult>> Handle(GetRepliesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            GetRepliesResult replies = await _commentRepository
                .GetReplies(query.VideoId, query.ParentCommentId, cancellationToken);

            return Result.Success(replies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting replies");
            throw;
        }
    }
}
