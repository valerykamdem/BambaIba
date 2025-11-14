using BambaIba.Domain.Comments;
using BambaIba.Infrastructure.Persistence;
using BambaIba.SharedKernel.Comments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BambaIba.Infrastructure.Repositories;
public class CommentRepository : ICommentRepository
{
    private readonly BambaIbaDbContext _dbContext;
    private readonly ILogger<CommentRepository> _logger;
    public CommentRepository(BambaIbaDbContext dbContext,
        ILogger<CommentRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task AddCommentAsync(Comment comment)
    {
        await _dbContext.Comments.AddAsync(comment);
        //_dbContext.SaveChangesAsync();
    }

    public void DeleteComment(Comment comment)
    {
        _dbContext.Comments.Remove(comment);
        _dbContext.SaveChangesAsync();
    }

    public async Task<Comment> GetComment(Guid commentId)
    {
        return await _dbContext.Comments.FindAsync(commentId);
    }

    public async Task<GetCommentsResult> GetComments(Guid videoId, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken)
    {
        IQueryable<Comment> query = _dbContext.Comments.AsQueryable()
                .Where(c => c.VideoId == videoId && c.ParentCommentId == null);

        int totalCount = await query.CountAsync(cancellationToken);

        List<CommentDto> comments = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                VideoId = c.VideoId,
                UserId = c.UserId,
                Content = c.Content,
                ParentCommentId = c.ParentCommentId,
                LikeCount = c.LikeCount,
                ReplyCount = _dbContext.Comments.Count(r => r.ParentCommentId == c.Id),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                IsEdited = c.IsEdited
            })
            .ToListAsync(cancellationToken);

        return new GetCommentsResult
        {
            Comments = comments,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task UpdateComment(Comment comment, CancellationToken cancellationToken)
    {
        if (comment == null)
        {
            throw new ArgumentNullException(nameof(comment), "Comment object cannot be null.");
        }

        _dbContext.Comments.Update(comment);

        // 3. Sauvegarder les changements dans la base de données
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> GetParentComment(
        Guid parentCommentId, 
        CancellationToken cancellationToken)
    {
        return await _dbContext.Comments
                    .AnyAsync(c => c.Id == parentCommentId, 
                    cancellationToken: cancellationToken);
    }

    public async Task<GetRepliesResult> GetReplies(Guid videoId, Guid parentCommentId, CancellationToken cancellationToken)
    {
        List<CommentDto> replies = await _dbContext.Comments
                .Where(c => c.VideoId == videoId
                         && c.ParentCommentId == parentCommentId)
                .OrderBy(c => c.CreatedAt) // Les réponses en ordre chronologique
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    VideoId = c.VideoId,
                    UserId = c.UserId,
                    Content = c.Content,
                    ParentCommentId = c.ParentCommentId,
                    LikeCount = c.LikeCount,
                    ReplyCount = 0, // Les réponses n'ont pas de sous-réponses
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    IsEdited = c.IsEdited
                })
                .ToListAsync(cancellationToken);

        return new GetRepliesResult
        {
            Replies = replies,
            TotalCount = replies.Count
        };
    }
}
