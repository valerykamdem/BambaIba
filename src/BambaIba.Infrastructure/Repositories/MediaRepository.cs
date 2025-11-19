using BambaIba.Domain.Comments;
using BambaIba.Domain.MediaBase;
using BambaIba.Infrastructure.Persistence;

namespace BambaIba.Infrastructure.Repositories;

public class MediaRepository : IMediaRepository
{
    private readonly BambaIbaDbContext _dbContext;

    public MediaRepository(BambaIbaDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task AddMediaAsync(Media media)
    {
       await _dbContext.Media.AddAsync(media);
        _dbContext.SaveChanges();
    }

    public async Task<Media> GetMediaById(Guid mediaId)
    {
        return await _dbContext.Media.FindAsync(mediaId);
    }
}
