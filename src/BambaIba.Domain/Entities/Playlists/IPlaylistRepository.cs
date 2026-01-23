namespace BambaIba.Domain.Entities.Playlists;
public interface IPlaylistRepository
{
    Task<Playlist?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Playlist playlist);
    Task UpdateAsync(Playlist playlist);
    Task DeleteAsync(Playlist playlist);
}
