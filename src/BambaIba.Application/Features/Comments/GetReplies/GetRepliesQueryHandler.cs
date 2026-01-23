using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Extensions;
using BambaIba.Domain.Entities.Comments;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Comments.GetReplies;
public sealed class GetRepliesQueryHandler : IQueryHandler<GetRepliesQuery, Result<PagedResult<CommentDto>>>
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

    public async Task<Result<PagedResult<CommentDto>>> Handle(GetRepliesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<Comment> replies = _commentRepository
                .GetReplies(query.VideoId, query.ParentCommentId, cancellationToken);

            PagedResult<CommentDto> pagedResult = await replies
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    MediaId = c.MediaId,
                    UserId = c.UserId,
                    Content = c.Content,
                    ParentCommentId = c.ParentCommentId,
                    LikeCount = c.LikeCount,
                    ReplyCount = 0, // Les réponses n'ont pas de sous-réponses
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    IsEdited = c.IsEdited
                })
                .ToPagedResultAsync(0,0, cancellationToken);

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
            _logger.LogError(ex, "Error getting replies");
            throw;
        }
    }
}
