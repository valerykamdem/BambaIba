using BambaIba.Domain.Videos;

namespace BambaIba.Domain.Likes;
public interface ILikeRepository
{
    void AddLike(Like like);
    
    Task<Like> GetLikeByUserAndVideoAsync(Guid userId, Guid videoId, 
        CancellationToken cancellationToken);
    
    void Delete(Like like);
}
