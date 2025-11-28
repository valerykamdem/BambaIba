using BambaIba.Domain.Likes;
using BambaIba.Domain.Videos;
using BambaIba.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BambaIba.Infrastructure.Repositories;
public class LikeRepository : ILikeRepository
{
    private readonly BambaIbaDbContext _dbContext;
    public LikeRepository(BambaIbaDbContext dbContext)
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

    public async Task<Like> GetLikeByUserAndVideoAsync(
        Guid userId, 
        Guid mediaId, 
        CancellationToken cancellationToken)
    {
        return await _dbContext.Likes
            .Where(l => l.UserId == userId && l.MediaId == mediaId)
            .FirstOrDefaultAsync(cancellationToken);
    }


}
