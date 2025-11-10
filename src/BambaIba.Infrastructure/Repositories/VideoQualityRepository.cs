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
        _dbContext.VideoQualities.Add(videoQuality);
        await _dbContext.SaveChangesAsync();
    }
}
