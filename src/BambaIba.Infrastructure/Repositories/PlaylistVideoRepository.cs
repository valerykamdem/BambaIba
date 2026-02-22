using BambaIba.Domain.Entities.PlaylistItems;
using BambaIba.Infrastructure.Persistence;

namespace BambaIba.Infrastructure.Repositories;
public class PlaylistVideoRepository : IPlaylistItemRepository
{
    private readonly BIDbContext _dbContext;
    public PlaylistVideoRepository(BIDbContext context)
    {
        _dbContext = context;
    }

    public async Task AddAsync(PlaylistItem playlistItem)
    {
        await _dbContext.PlaylistItems.AddAsync(playlistItem);
        await _dbContext.SaveChangesAsync();
    }

    public Task DeleteAsync(PlaylistItem playlistItem)
    {
        throw new NotImplementedException();
    }

    public Task<PlaylistItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(PlaylistItem playlistItem)
    {
        throw new NotImplementedException();
    }
}
