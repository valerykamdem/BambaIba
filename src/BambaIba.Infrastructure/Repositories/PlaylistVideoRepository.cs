using BambaIba.Domain.PlaylistVideos;
using BambaIba.Infrastructure.Persistence;

namespace BambaIba.Infrastructure.Repositories;
public class PlaylistVideoRepository : IPlaylistVideoRepository
{
    private readonly BambaIbaDbContext _context;
    public PlaylistVideoRepository(BambaIbaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PlaylistVideo playlistVideo)
    {
        _context.PlaylistVideos.Add(playlistVideo);
        await _context.SaveChangesAsync();
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
