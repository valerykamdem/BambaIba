using BambaIba.Application.Common.Interfaces;
using BambaIba.Domain.Entities;
using BambaIba.Infrastructure.Persistence;

namespace BambaIba.Infrastructure.Services;
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
