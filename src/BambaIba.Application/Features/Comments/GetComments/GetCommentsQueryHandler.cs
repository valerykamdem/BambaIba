using BambaIba.Application.Extensions;
using BambaIba.Domain.Comments;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Comments;
using Cortex.Mediator.Queries;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Comments.GetComments;
public class GetCommentsQueryHandler : IQueryHandler<GetCommentsQuery, Result<PagedResult<CommentDto>>>
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

    public async Task<Result<PagedResult<CommentDto>>> Handle(
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

            IQueryable<Comment> comments = _commentRepository.GetComments(query.VideoId, cancellationToken);

            PagedResult<CommentDto> pagedResult = await comments
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    VideoId = c.VideoId,
                    UserId = c.UserId,
                    Content = c.Content,
                    ParentCommentId = c.ParentCommentId,
                    LikeCount = c.LikeCount,
                    //ReplayCount = _dbContext.Comments.Count(r => r.ParentCommentId == c.Id),
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    IsEdited = c.IsEdited
                })
                .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

            return Result.Success(new PagedResult<CommentDto>
            {
                Items = pagedResult.Items,
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages
            });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments");
            throw;
        }
    }
}
