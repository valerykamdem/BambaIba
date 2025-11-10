
using BambaIba.SharedKernel.Videos;

namespace BambaIba.Domain.Videos;

public interface IVideoRepository
{
    void AddVideo(Video video);
    Task<Video> GetVideoById(Guid videoId);
    Task<VideoDetailResult> GetVideoDetailResultById(Guid videoId, CancellationToken cancellationToken);
    Task UpdateVideoStatus(Video video);
    Task<GetVideosResult> GetVideos(int page, int pageSize, string search, CancellationToken cancellationToken);
    void Delete(Video video);
}
