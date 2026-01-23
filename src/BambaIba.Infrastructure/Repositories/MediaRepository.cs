using BambaIba.Domain.Entities.MediaBase;
using BambaIba.Domain.Entities.Videos;
using BambaIba.Domain.Enums;
using BambaIba.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

    public async Task DeleteAsync(Media media)
    {
        _dbContext.Media.Remove(media);
       await _dbContext.SaveChangesAsync();
    }

    public async Task<Media?> GetMediaByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Récupérer le média générique
        //Media? media = await _dbContext.Media
        //    .Where(m => m.Id == id && m.Status == MediaStatus.Ready)
        //    .FirstOrDefaultAsync(cancellationToken);
        //return media;
        return await _dbContext.Media.FindAsync([id], cancellationToken: cancellationToken);
    }

    public IQueryable<Media> GetMediaAsync()
    {
        return _dbContext.Media.AsQueryable()
            .Where(m => m.Status == MediaStatus.Ready && m.IsPublic);
    }

    public async Task<Media?> GetMediaWithDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        // 1. Récupérer les métadonnées depuis PostgreSQL
        Media? media = await _dbContext.Media
            //.Include(v => v.Qualities)
            .Where(v => v.Id == id && v.Status == MediaStatus.Ready)
            .FirstOrDefaultAsync(cancellationToken);

        if (media == null)
            return null;

        // Si c’est une vidéo, charger les Qualities
        if (media is Video video)
        {
            await _dbContext.Entry(video)
                .Collection(v => v.Qualities)
                .LoadAsync(cancellationToken);
        }

        // 4. Enregistrer la vue (asynchrone)
        media.PlayCount++;
        _dbContext.Entry(media).Property(v => v.PlayCount).IsModified = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return media;
    }

    public Task UpdateAsync(Media media)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateMediaStatus(Media media)
    {
        if (media == null)
        {
            throw new ArgumentNullException(nameof(media), "Media object cannot be null.");
        }

        // Vérifie si l'entité est déjà suivie
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Media>? tracked = _dbContext.ChangeTracker.Entries<Media>()
                              .FirstOrDefault(e => e.Entity.Id == media.Id);

        if (tracked == null)
        {
            // Attache l'entité si elle est détachée
            _dbContext.Attach(media);
        }

        // 2. Marquer les propriétés que vous avez modifiées.
        // C'est plus efficace que de charger l'objet une deuxième fois.

        // Champs que vous modifiez dans ProcessVideoAsync :
        _dbContext.Entry(media).Property(v => v.Duration).IsModified = true;
        _dbContext.Entry(media).Property(v => v.ThumbnailPath).IsModified = true;
        _dbContext.Entry(media).Property(v => v.Status).IsModified = true;
        _dbContext.Entry(media).Property(v => v.PublishedAt).IsModified = true;
        _dbContext.Entry(media).Property(v => v.UpdatedAt).IsModified = true;
        _dbContext.Entry(media).Property(v => v.StoragePath).IsModified = true;

        // 3. Sauvegarder les changements dans la base de données
        await _dbContext.SaveChangesAsync();
    }
}
