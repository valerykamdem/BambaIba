namespace BambaIba.Domain.PlaylistVideos;
public interface IPlaylistVideoRepository
{
    Task<PlaylistVideo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PlaylistVideo playlistVideo);
    Task UpdateAsync(PlaylistVideo playlistVideo);
    Task DeleteAsync(PlaylistVideo playlistVideo);
}
