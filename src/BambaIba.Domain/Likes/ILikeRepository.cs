using BambaIba.Domain.Videos;

namespace BambaIba.Domain.Likes;
public interface ILikeRepository
{
    Task AddLikeAsync(Like like);
    
    Task<Like> GetLikeByUserAndVideoAsync(Guid userId, Guid videoId, 
        CancellationToken cancellationToken);
    
    void Delete(Like like);
}
