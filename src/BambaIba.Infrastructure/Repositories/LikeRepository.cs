using BambaIba.Domain.Entities.Likes;
using BambaIba.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BambaIba.Infrastructure.Repositories;
public class LikeRepository : ILikeRepository
{
    private readonly BIDbContext _dbContext;
    public LikeRepository(BIDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddLikeAsync(Like like)
    {
        await _dbContext.Likes.AddAsync(like);
    }

    public void Delete(Like like)
    {
        _dbContext.Likes.Remove(like);
    }

    public async Task<Like> GetLikeByUserAndMediaAsync(
        Guid userId, 
        Guid mediaId, 
        CancellationToken cancellationToken)
    {
        return await _dbContext.Likes
            .Where(l => l.UserId == userId && l.MediaId == mediaId)
            .FirstOrDefaultAsync(cancellationToken);
    }


}
