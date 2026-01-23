namespace BambaIba.Domain.Entities.Likes;
public interface ILikeRepository
{
    Task AddLikeAsync(Like like);
    
    Task<Like> GetLikeByUserAndMediaAsync(Guid userId, Guid mediaId, 
        CancellationToken cancellationToken);
    
    void Delete(Like like);
}
