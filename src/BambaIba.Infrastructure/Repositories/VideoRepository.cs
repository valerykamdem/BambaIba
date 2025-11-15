using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Enums;
using BambaIba.Domain.VideoQualities;
using BambaIba.Domain.Videos;
using BambaIba.Infrastructure.Persistence;
using BambaIba.SharedKernel.Videos;
using Microsoft.EntityFrameworkCore;

namespace BambaIba.Infrastructure.Repositories;
public class VideoRepository : IVideoRepository
{
    private readonly BambaIbaDbContext _dbContext;
    private readonly IMediaStorageService _storageService;

    public VideoRepository(BambaIbaDbContext dbContext, IMediaStorageService storageService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
    }

    public async Task AddVideoAsync(Video video)
    {
        await _dbContext.Videos.AddAsync(video);
        //await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Video> GetVideoById(Guid videoId)
    {
        return await _dbContext.Videos.FindAsync(videoId);
    }

    public async Task<Video> GetVideoWithQualitiesAsync(Guid videoId, CancellationToken cancellationToken)
    {

        // 1. Récupérer les métadonnées depuis PostgreSQL
        Video video = await _dbContext.Videos
            //.Include(v => v.VideoQualities)
            .Where(v => v.Id == videoId && v.Status == MediaStatus.Ready)
            .FirstOrDefaultAsync(cancellationToken);

        if (video == null)
            return null;

        // 4. Enregistrer la vue (asynchrone)
        video.ViewCount++;
        _dbContext.Entry(video).Property(v => v.ViewCount).IsModified = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return video;
        
    }
    public IQueryable<Video> GetVideos()
    {
        return _dbContext.Videos.AsQueryable()
            .Where(v => v.Status == MediaStatus.Ready && v.IsPublic);
    }

    // 🚨 Code de la méthode UpdateVideoStatus 🚨
    public async Task UpdateVideoStatus(Video video)
    {
        if (video == null)
        {
            throw new ArgumentNullException(nameof(video), "Video object cannot be null.");
        }

        // Vérifie si l'entité est déjà suivie
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Video>? tracked = _dbContext.ChangeTracker.Entries<Video>()
                              .FirstOrDefault(e => e.Entity.Id == video.Id);

        if (tracked == null)
        {
            // Attache l'entité si elle est détachée
            _dbContext.Attach(video);
        }

        // 2. Marquer les propriétés que vous avez modifiées.
        // C'est plus efficace que de charger l'objet une deuxième fois.

        // Champs que vous modifiez dans ProcessVideoAsync :
        _dbContext.Entry(video).Property(v => v.Duration).IsModified = true;
        _dbContext.Entry(video).Property(v => v.ThumbnailPath).IsModified = true;
        _dbContext.Entry(video).Property(v => v.Status).IsModified = true;
        _dbContext.Entry(video).Property(v => v.PublishedAt).IsModified = true;
        _dbContext.Entry(video).Property(v => v.UpdatedAt).IsModified = true;
        _dbContext.Entry(video).Property(v => v.StoragePath).IsModified = true;

        // Si d'autres propriétés comme 'Title' ont pu être changées, 
        // ajoutez-les ici ou utilisez la méthode Update() plus simple.

        // 3. Sauvegarder les changements dans la base de données
        await _dbContext.SaveChangesAsync();
    }

    public void Delete(Video video)
    {
        _dbContext.Videos.Remove(video);
        _dbContext.SaveChangesAsync();
    }
}
