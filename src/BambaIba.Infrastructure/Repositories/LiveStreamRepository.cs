using System.Threading;
using BambaIba.Application.Features.LiveStreams.GetLiveStreams;
using BambaIba.Domain.Entities.LiveStream;
using BambaIba.Domain.Enums;
using BambaIba.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BambaIba.Infrastructure.Repositories;
public class LiveStreamRepository : ILiveStreamRepository
{
    private readonly BambaIbaDbContext _dbContext;
    public LiveStreamRepository(BambaIbaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(LiveStream liveStream)
    {
        await _dbContext.LiveStreams.AddAsync(liveStream);
        await _dbContext.SaveChangesAsync();
    }

    public Task DeleteAsync(LiveStream liveStream)
    {
        throw new NotImplementedException();
    }

    public Task<LiveStream?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<LiveStream?> GetByStreamId(Guid streamId, 
        CancellationToken cancellationToken)
    {
        return await _dbContext.LiveStreams
            .FirstOrDefaultAsync(s => s.Id == streamId, cancellationToken);
    }

    public IQueryable<LiveStream?> GetLiveStream()
    {
        // Returns the first LiveStream or null if none exist
        return _dbContext.LiveStreams.AsQueryable();
    }

    public Task UpdateAsync(LiveStream liveStream)
    {
        throw new NotImplementedException();
    }
}
