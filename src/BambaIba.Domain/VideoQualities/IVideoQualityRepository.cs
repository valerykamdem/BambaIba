namespace BambaIba.Domain.VideoQualities;

public interface IVideoQualityRepository
{
    Task AddVideoQuality(VideoQuality videoQuality);
    IQueryable<VideoQuality> GetAllByMediaId(Guid mediaId);
}
