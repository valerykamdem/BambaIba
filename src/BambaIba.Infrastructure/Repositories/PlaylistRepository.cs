using System.Threading;
using BambaIba.Domain.Playlists;
using BambaIba.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BambaIba.Infrastructure.Repositories;
public class PlaylistRepository : IPlaylistRepository
{
    private readonly BambaIbaDbContext _dbContext;
    public PlaylistRepository(BambaIbaDbContext context)
    {
        _dbContext = context;
    }

    public async Task AddAsync(Playlist playlist)
    {
        await _dbContext.Playlists.AddAsync(playlist);
        await _dbContext.SaveChangesAsync();
    }

    public Task DeleteAsync(Playlist playlist)
    {
        throw new NotImplementedException();
    }

    public async Task<Playlist?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Playlists
                 .Include(p => p.Items)
                 .ThenInclude(pv => pv.Media)
                 .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task UpdateAsync(Playlist playlist)
    {
        throw new NotImplementedException();
    }
}
