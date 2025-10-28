
using System.Threading.Tasks;
using BambaIba.Domain.Entities;

namespace BambaIba.Application.Common.Interfaces;
public interface IVideoRepository
{
    Task AddVideo(Video video, CancellationToken cancellationToken);
    Task<Video> GetVideoById(Guid videoId);
    Task UpdateVideoStatus(Video video);
}
