using BambaIba.Domain.Comments;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Comments;
using Cortex.Mediator.Queries;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Comments.GetComments;
public class GetCommentsQueryHandler : IQueryHandler<GetCommentsQuery, Result<GetCommentsResult>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<GetCommentsQueryHandler> _logger;

    public GetCommentsQueryHandler(
        ICommentRepository commentRepository,
        ILogger<GetCommentsQueryHandler> logger)
    {
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async Task<Result<GetCommentsResult>> Handle(
        GetCommentsQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
               "Getting Comments: Page={Page}, PageSize={PageSize}, VideoId={VideoId}",
               query.Page,
               query.PageSize,
               query.VideoId);

            Task<GetCommentsResult> comments = 
                _commentRepository.GetComments(
                    query.VideoId,
                    query.Page,
                    query.PageSize,
                    cancellationToken);

            return Result.Success(await comments);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments");
            throw;
        }
    }
}
