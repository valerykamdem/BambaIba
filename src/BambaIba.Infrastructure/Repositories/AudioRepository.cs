using BambaIba.Domain.Audios;
using BambaIba.Domain.Enums;
using BambaIba.Infrastructure.Persistence;

namespace BambaIba.Infrastructure.Repositories;
public class AudioRepository : IAudioRepository
{
    private readonly BambaIbaDbContext _dbContext;
    public AudioRepository(BambaIbaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Audio audio)
    {
        await _dbContext.Audios.AddAsync(audio);
        await _dbContext.SaveChangesAsync();
    }

    public Task DeleteAsync(Audio audio)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Audio> GetAudioAsync()
    {
       return _dbContext.Audios.AsQueryable()
            .Where(a => a.Status == MediaStatus.Ready && a.IsPublic);
    }

    public async Task<Audio?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Audio audio = await _dbContext.Audios
            .FindAsync([id, cancellationToken], cancellationToken: cancellationToken);

        if (audio is null)
            return null;

        audio.PlayCount++;
        _dbContext.Entry(audio).Property(a => a.PlayCount).IsModified = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return audio;
    }

    public Task<List<Audio>> GetPagedAsync(int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Audio audio)
    {
        _dbContext.Audios.Update(audio);
        await _dbContext.SaveChangesAsync();
    }
}
