using System.Threading;
using BambaIba.Domain.Playlists;
using BambaIba.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BambaIba.Infrastructure.Repositories;
public class PlaylistRepository : IPlaylistRepository
{
    private readonly BambaIbaDbContext _context;
    public PlaylistRepository(BambaIbaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Playlist playlist)
    {
        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Playlist playlist)
    {
        throw new NotImplementedException();
    }

    public async Task<Playlist?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Playlists
                 .Include(p => p.Videos)
                 .ThenInclude(pv => pv.Video)
                 .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task UpdateAsync(Playlist playlist)
    {
        throw new NotImplementedException();
    }
}
