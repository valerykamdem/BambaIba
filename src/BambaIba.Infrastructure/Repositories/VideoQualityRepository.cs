using BambaIba.Domain.VideoQualities;
using BambaIba.Infrastructure.Persistence;

namespace BambaIba.Infrastructure.Repositories;
public class VideoQualityRepository : IVideoQualityRepository
{
    private readonly BambaIbaDbContext _dbContext;

    public VideoQualityRepository(BambaIbaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddVideoQuality(VideoQuality videoQuality)
    {
        await _dbContext.VideoQualities.AddAsync(videoQuality);
        await _dbContext.SaveChangesAsync();
    }

    public IQueryable<VideoQuality> GetAllByVideoId(Guid videoId)
    {
       return _dbContext.VideoQualities.AsQueryable()
            .Where(q => q.VideoId == videoId);
    }
}
