
namespace BambaIba.Domain.LiveStream;
public interface ILiveStreamRepository
{
    IQueryable<LiveStream?> GetLiveStream();
    Task<LiveStream?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(LiveStream liveStream);
    Task UpdateAsync(LiveStream liveStream);
    Task DeleteAsync(LiveStream liveStream);
    Task<LiveStream?> GetByStreamId(Guid streamId, CancellationToken cancellationToken = default);
}
