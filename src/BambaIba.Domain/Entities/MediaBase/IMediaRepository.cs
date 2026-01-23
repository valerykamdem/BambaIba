namespace BambaIba.Domain.Entities.MediaBase;

public interface IMediaRepository
{
    Task AddMediaAsync(Media media);
    Task<Media?> GetMediaByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Media?> GetMediaWithDetailsAsync(Guid id, CancellationToken cancellationToken);
    IQueryable<Media> GetMediaAsync();
    Task UpdateMediaStatus(Media media);
    Task UpdateAsync(Media media);
    Task DeleteAsync(Media media);
}
