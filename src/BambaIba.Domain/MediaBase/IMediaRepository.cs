namespace BambaIba.Domain.MediaBase;

public interface IMediaRepository
{
    Task AddMediaAsync(Media media);
    Task<Media> GetMediaById(Guid mediaId);
}
