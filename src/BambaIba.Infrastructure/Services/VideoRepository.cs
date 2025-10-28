using BambaIba.Application.Common.Interfaces;
using BambaIba.Domain.Entities;
using BambaIba.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BambaIba.Infrastructure.Services;
public class VideoRepository : IVideoRepository
{
    private readonly BambaIbaDbContext _dbContext;

    public VideoRepository(BambaIbaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddVideo(Video video, CancellationToken cancellationToken)
    {
        _dbContext.Videos.Add(video);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Video> GetVideoById(Guid videoId)
    {
        Video video = await _dbContext.Videos.FindAsync(videoId);

        return video;
    }

    // 🚨 Code de la méthode UpdateVideoStatus 🚨
    public async Task UpdateVideoStatus(Video video)
    {
        if (video == null)
        {
            throw new ArgumentNullException(nameof(video), "Video object cannot be null.");
        }

        // 1. Attacher l'objet au contexte actuel (créé par le scope)
        // Ceci est crucial car l'objet 'video' vient d'un ancien contexte.
        _dbContext.Videos.Attach(video);

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

    //// 🚨 Code de la méthode UpdateVideoStatus 🚨
    //public async Task UpdateVideoStatus(Video video)
    //{
    //    if (video == null)
    //    {
    //        throw new ArgumentNullException(nameof(video), "Video object cannot be null.");
    //    }

    //    // 1. Attacher l'objet au contexte actuel (créé par le scope)
    //    // Ceci est crucial car l'objet 'video' vient d'un ancien contexte.
    //    _dbContext.Videos.Attach(video);

    //    // 2. Marquer les propriétés que vous avez modifiées.
    //    // C'est plus efficace que de charger l'objet une deuxième fois.

    //    // Champs que vous modifiez dans ProcessVideoAsync :
    //    _dbContext.Entry(video).Property(v => v.Duration).IsModified = true;
    //    _dbContext.Entry(video).Property(v => v.ThumbnailPath).IsModified = true;
    //    _dbContext.Entry(video).Property(v => v.Status).IsModified = true;
    //    _dbContext.Entry(video).Property(v => v.PublishedAt).IsModified = true;
    //    _dbContext.Entry(video).Property(v => v.UpdatedAt).IsModified = true;

    //    // Si d'autres propriétés comme 'Title' ont pu être changées, 
    //    // ajoutez-les ici ou utilisez la méthode Update() plus simple.

    //    // 3. Sauvegarder les changements dans la base de données
    //    await _dbContext.SaveChangesAsync();
    //}

}
