namespace BambaIba.Domain.PlaylistItems;
public interface IPlaylistItemRepository
{
    Task<PlaylistItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PlaylistItem playlistItem);
    Task UpdateAsync(PlaylistItem playlistItem);
    Task DeleteAsync(PlaylistItem playlistItem);
}
