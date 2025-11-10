using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.VideoQualities;
using BambaIba.Domain.Videos;
using BambaIba.Infrastructure.Persistence;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Enums;
using BambaIba.SharedKernel.Videos;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BambaIba.Infrastructure.Repositories;
public class VideoRepository : IVideoRepository
{
    private readonly BambaIbaDbContext _dbContext;
    private readonly IVideoStorageService _storageService;

    public VideoRepository(BambaIbaDbContext dbContext, IVideoStorageService storageService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
    }

    public void AddVideo(Video video)
    {
        _dbContext.Videos.Add(video);
        //await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Video> GetVideoById(Guid videoId)
    {
        return await _dbContext.Videos.FindAsync(videoId);
    }

    public async Task<VideoDetailResult> GetVideoDetailResultById(Guid videoId, CancellationToken cancellationToken)
    {

        // 1. Récupérer les métadonnées depuis PostgreSQL
        Video video = await _dbContext.Videos
            .Where(v => v.Id == videoId && v.Status == VideoStatus.Ready)
            .FirstOrDefaultAsync(cancellationToken);

        if (video == null)
            return null;

        // 2. Récupérer les qualités disponibles
        List<VideoQuality> qualities = await _dbContext.VideoQualities
            .Where(q => q.VideoId == videoId)
            .ToListAsync(cancellationToken);

        //// 3. Générer des URLs présignées (valides 1h)
        //string videoUrl = await _storageService.GetPresignedUrlAsync(
        //    video.StoragePath,
        //    expiryInSeconds: 3600);

        // Appel du service
        string videoUrl = await _storageService.GetVideoUrlAsync(
            video.StoragePath,
            video.IsPublic);

        //string thumbnailUrl = await _storageService.GetPresignedUrlAsync(
        //    video.ThumbnailPath,
        //    expiryInSeconds: 3600);

        string thumbnailUrl = _storageService.GetPublicUrl(video.ThumbnailPath);

        var qualityDtos = new List<VideoQualityDto>();
        foreach (VideoQuality quality in qualities)
        {
            // Signed
            string qualityUrl = await _storageService.GetPresignedUrlAsync(
                quality.StoragePath,
                expiryInSeconds: 3600);

            qualityDtos.Add(new VideoQualityDto
            {
                Quality = quality.Quality,
                //Url = qualityUrl
                Url = quality.StoragePath
            });
        }

        // 4. Enregistrer la vue (asynchrone)
        //_ = Task.Run(async () => await RecordViewAsync(videoId), cancellationToken);
        video.ViewCount++;
        _dbContext.Entry(video).Property(v => v.ViewCount).IsModified = true;
        //await _dbContext.SaveChangesAsync(cancellationToken);

        return new VideoDetailResult
        {
            Id = video.Id,
            Title = video.Title,
            Description = video.Description,
            VideoUrl = videoUrl,           // ← URL temporaire
            ThumbnailUrl = thumbnailUrl,   // ← URL temporaire
            Duration = video.Duration,
            ViewCount = video.ViewCount,
            Qualities = qualityDtos
        };
    }

    //private async Task RecordViewAsync(Guid videoId)
    //{
    //    Video video = await _dbContext.Videos.FindAsync(videoId);
    //    if (video != null)
    //    {
    //        video.ViewCount++;
    //        await _dbContext.SaveChangesAsync();
    //    }
    //}

    public async Task<GetVideosResult> GetVideos(int page, int pageSize, string search, CancellationToken cancellationToken)
    {
        // ✅ Query PostgreSQL uniquement - RAPIDE
        List<VideoDto> videos = await _dbContext.Videos
            .Where(v => v.Status == VideoStatus.Ready && v.IsPublic)
            .OrderByDescending(v => v.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new VideoDto
            {
                Id = v.Id,
                Title = v.Title,
                Description = v.Description,
                ThumbnailUrl = v.ThumbnailPath,  // ← Juste le chemin, pas le fichier
                Duration = v.Duration,
                ViewCount = v.ViewCount,
                LikeCount = v.LikeCount,
                CreatedAt = (DateTime)v.CreatedAt!,
                UserId = v.UserId,
            })
            .ToListAsync(cancellationToken);

        int totalPages = (int)Math.Ceiling(videos.Count / (double)pageSize);

        return new GetVideosResult(
                videos,
                videos.Count,
                page,
                pageSize,
                totalPages
            );
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
