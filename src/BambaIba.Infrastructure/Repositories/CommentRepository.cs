using BambaIba.Domain.Comments;
using BambaIba.Infrastructure.Persistence;
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

    public IQueryable<Comment> GetComments(Guid videoId,
        CancellationToken cancellationToken)
    {
        IQueryable<Comment> query = _dbContext.Comments.AsQueryable()
                .Where(c => c.VideoId == videoId && c.ParentCommentId == null);

        return query;
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

    public IQueryable<Comment> GetReplies(Guid videoId, Guid parentCommentId, CancellationToken cancellationToken)
    {
        return _dbContext.Comments.AsQueryable()
            .Where(c => c.VideoId == videoId
                         && c.ParentCommentId == parentCommentId);
    }
}
