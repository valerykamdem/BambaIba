using BambaIba.Domain.PlaylistVideos;
using BambaIba.Infrastructure.Persistence;

namespace BambaIba.Infrastructure.Repositories;
public class PlaylistVideoRepository : IPlaylistVideoRepository
{
    private readonly BambaIbaDbContext _dbContext;
    public PlaylistVideoRepository(BambaIbaDbContext context)
    {
        _dbContext = context;
    }

    public async Task AddAsync(PlaylistVideo playlistVideo)
    {
        await _dbContext.PlaylistVideos.AddAsync(playlistVideo);
        await _dbContext.SaveChangesAsync();
    }

    public Task DeleteAsync(PlaylistVideo playlistVideo)
    {
        throw new NotImplementedException();
    }

    public Task<PlaylistVideo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(PlaylistVideo playlistVideo)
    {
        throw new NotImplementedException();
    }
}
