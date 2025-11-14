
namespace BambaIba.Domain.Audios;
public interface IAudioRepository
{
    Task<Audio?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    IQueryable<Audio> GetAudioAsync();
    Task AddAsync(Audio audio);
    Task UpdateAsync(Audio audio);
    Task DeleteAsync(Audio audio);
    Task<List<Audio>> GetPagedAsync(int pageNumber, int pageSize);
}
