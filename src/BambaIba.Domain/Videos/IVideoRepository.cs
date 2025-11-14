namespace BambaIba.Domain.Videos;

public interface IVideoRepository
{
    Task AddVideoAsync(Video video);
    Task<Video> GetVideoById(Guid videoId);
    Task<Video> GetVideoWithQualitiesAsync(Guid videoId, CancellationToken cancellationToken);
    //Task<VideoDetailResult> GetVideoDetailResultById(Guid videoId, CancellationToken cancellationToken);
    Task UpdateVideoStatus(Video video);
    IQueryable<Video> GetVideos();
    void Delete(Video video);
}
