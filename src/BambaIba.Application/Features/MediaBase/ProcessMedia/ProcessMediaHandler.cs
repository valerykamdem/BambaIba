using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Features.Searchs;
using BambaIba.Application.Settings;
using BambaIba.Domain.Entities.Audios;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.VideoQualities;
using BambaIba.Domain.Entities.Videos;
using BambaIba.Domain.Enums;
using DnsClient.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BambaIba.Application.Features.MediaBase.ProcessMedia;

public sealed record ProcessMediaCommand(Guid MediaId, Guid UserId);

public sealed class ProcessMediaHandler(
        IBIDbContext dbContext,
        IMediaStorageService storageService,
        IMediaProcessingService processingService,
        IMessageBus bus,
        ILogger<ProcessMediaHandler> logger)
{
    public async Task Handle(ProcessMediaCommand command, CancellationToken cancellationToken)
    {
        // 1. Récupération du média
        MediaAsset? media = await dbContext.MediaAssets
            .FirstOrDefaultAsync(m => m.Id == command.MediaId, cancellationToken);

        if (media == null)
            return;

        // 2. IDEMPOTENCE : Si déjà Ready, on arrête
        if (media.Status == MediaStatus.Ready)
        {
            logger.LogInformation("Media {Id} is already Ready.", media.Id);
            return;
        }

        // 3. DETECTION "ZOMBIE" : Si les qualités existent déjà, on répare le statut
        // On vérifie AVANT tout traitement lourd
        int existingCount = await dbContext.VideoQualities
            .CountAsync(q => q.MediaId == media.Id, cancellationToken);

        if (existingCount > 0)
        {
            logger.LogWarning("Media {Id} has existing qualities but status is {Status}. Fixing to Ready.", media.Id, media.Status);

            media.Status = MediaStatus.Ready;
            media.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
            return; // Le travail est déjà fait, on sort.
        }

        string localSourcePath = await storageService.DownloadVideoAsync(media.StoragePath, cancellationToken);

        // 4. Mise à jour du statut
        media.Status = MediaStatus.Processing;
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            // 5. Traitement (SÉQUENTIEL pour éviter les bugs DbContext)
            // On enlève le Task.WhenAll et le Task.Run
            if (media is Video v)
            {
                // Appel séquentiel pour chaque qualité
                foreach (string quality in VideoQualitySetting.All)
                {
                    await ProcessSingleQualityAsync(command.UserId, v, localSourcePath, quality, cancellationToken);
                }
            }
            else if (media is Audio a)
            {
                await ProcessAudioAsync(a, cancellationToken);
            }

            // 6. Succès
            media.Status = MediaStatus.Ready;
            media.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);

            // Indexation...
            try
            { await bus.PublishAsync(new IndexMediaCommand(media.Id)); }
            catch { /* Ignorer */ }
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true)
        {
            // 7. CAS PARTICULIER : Duplicate Key
            // Si on arrive ici, ça veut dire qu'un autre processus a inséré les données entre-temps.
            // On considère que c'est un SUCCÈS (le travail est fait).
            logger.LogWarning("Duplicate key detected for {Id}. Assuming processing is already done.", media.Id);

            // On rafraîchit l'entité pour éviter les conflits de tracking
            // Et on force le statut Ready
            MediaAsset? freshMedia = await dbContext.MediaAssets.FindAsync(command.MediaId);
            if (freshMedia != null)
            {
                freshMedia.Status = MediaStatus.Ready;
                freshMedia.UpdatedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Processing failed {Id}", command.MediaId);

            // Nettoyer les qualités partielles en cas d'échec réel
            List<VideoQuality> partials = await dbContext.VideoQualities.Where(q => q.MediaId == media.Id).ToListAsync();
            if (partials.Any())
            {
                dbContext.VideoQualities.RemoveRange(partials);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            media.Status = MediaStatus.Failed;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    // Nouvelle méthode helper pour traiter une seule qualité (propre)
    private async Task ProcessSingleQualityAsync(Guid userId, Video video, string localSourcePath, string quality, CancellationToken ct)
    {
        // 1. IDEMPOTENCE : Vérifier si cette qualité existe déjà en base
        // Cela évite le crash "Duplicate Key" si le handler est rejoué
        bool exists = await dbContext.VideoQualities
            .AnyAsync(q => q.MediaId == video.Id && q.Quality == quality, ct);

        if (exists)
        {
            logger.LogInformation("Quality {Quality} already exists for Media {Id}. Skipping creation.", quality, video.Id);
            return;
        }

        // 2. TRANSCODAGE (Appel au service FFmpeg)
        // On passe le fichier local déjà téléchargé
        string storageKey = await processingService.TranscodeVideoAsync(
            video.Id,
            localSourcePath, // Important : le fichier local source
            quality,
            ct
        );

        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new Exception($"Transcode returned empty storage key for quality {quality}");
        }

        // 3. RECUPERATION TAILLE (S3/SeaweedFS)
        long size = await storageService.GetFileSizeAsync(storageKey, ct);

        // 4. CREATION ENTITE
        var videoQuality = new VideoQuality
        {
            Id = Guid.CreateVersion7(),
            MediaId = video.Id,
            Quality = quality,
            StoragePath = storageKey,
            FileSize = size,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString()
        };

        // 5. AJOUT AU CONTEXTE
        // Note: Le SaveChanges est géré par la méthode Handle appelante (après la boucle)
        dbContext.VideoQualities.Add(videoQuality);
    }

    // Helper pour détecter les erreurs temporaires (Réseau, Timeout S3, etc.)
    private bool IsTransientError(Exception ex)
    {
        return ex is TimeoutException
            || ex is TaskCanceledException
            || ex is HttpRequestException;
            //|| ex is /*AmazonS3Exception s3Ex &&*/ s3Ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable;
    }

    //public async Task Handle(
    //ProcessMediaCommand command,
    //CancellationToken cancellationToken)
    //{

    //    MediaAsset? media =
    //        await dbContext.MediaAssets
    //            .FirstOrDefaultAsync(m => m.Id == command.MediaId, cancellationToken: cancellationToken);

    //    if (media == null) return;

    //    try
    //    {
    //        // Phase 1
    //        media.Status = MediaStatus.Processing;
    //        await dbContext.SaveChangesAsync(CancellationToken.None);

    //        // Phase 2
    //        if (media is Video v)
    //            await ProcessVideoAsync(command.UserId, v, cancellationToken);

    //        else if (media is Audio a)
    //            await ProcessAudioAsync(a, cancellationToken);

    //        // Phase 3
    //        media.Status = MediaStatus.Ready;
    //        media.UpdatedAt = DateTime.UtcNow;

    //        await dbContext.SaveChangesAsync(CancellationToken.None);

    //        // on lance l'indexation !
    //        try
    //        {
    //            await bus.PublishAsync(new IndexMediaCommand(media.Id));
    //        }
    //        catch (Exception ex)
    //        {
    //            // On log l'erreur mais on ne plante pas le processus
    //            logger.LogError(ex, "Failed to index media {Id}, but media is ready.", media.Id);
    //        }

    //        logger.LogInformation("Media {Id} done", media.Id);
    //    }
    //    catch (OperationCanceledException)
    //    {
    //        logger.LogWarning("Processing canceled {Id}", command.MediaId);

    //        media.Status = MediaStatus.Pending;
    //        await dbContext.SaveChangesAsync(CancellationToken.None);
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, "Processing failed {Id}", command.MediaId);

    //        media.Status = MediaStatus.Failed;
    //        await dbContext.SaveChangesAsync(CancellationToken.None);
    //    }
    //}

    private async Task ProcessVideoAsync(Guid UserId, Video video, CancellationToken ct)
    {

        // 1. Télécharger la source une seule fois
        string localSourcePath =
            await storageService.DownloadVideoAsync(video.StoragePath, ct);

        try
        {
            // 2. Durée
            video.Duration =
                await processingService.GetDurationAsync(localSourcePath);

            // 3. Thumbnail si manquant
            if (string.IsNullOrEmpty(video.ThumbnailPath))
            {
                video.ThumbnailPath =
                    await processingService.GenerateThumbnailAsync(video.Id, localSourcePath);
            }

            // 4. Transcodage parallélisé des qualités
            var throttler = new SemaphoreSlim(4); // ex: max 4 qualités en parallèle
            var tasks = new List<Task>();

            foreach (string quality in VideoQualitySetting.All)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await throttler.WaitAsync(ct);
                    try
                    {
                        string storageKey = await processingService.TranscodeVideoAsync(video.Id, localSourcePath, quality, ct);

                        if (string.IsNullOrWhiteSpace(storageKey))
                        {
                            throw new Exception("Transcode returned empty storageKey");
                        }

                        long size = await storageService.GetFileSizeAsync(storageKey, ct);


                        dbContext.VideoQualities.Add(new VideoQuality
                        {
                            Id = Guid.CreateVersion7(),
                            MediaId = video.Id,
                            Quality = quality,
                            StoragePath = storageKey,
                            FileSize = size,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = UserId.ToString() ?? "system"
                        });

                    }
                    finally
                    {
                        throttler.Release();
                    }
                }, ct));
            }

            await Task.WhenAll(tasks);
        }
        finally
        {
            if (File.Exists(localSourcePath))
            {
                await TryDeleteFileAsync(localSourcePath);
            }
        }
    }

    private async Task TryDeleteFileAsync(string path, int retries = 3)
    {
        for (int i = 0; i < retries; i++)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
                return; // Succès
            }
            catch (IOException)
            {
                // Le fichier est vérouillé, on attend un peu et on réessaie
                await Task.Delay(100 * (i + 1)); // Attend 100ms, puis 200ms, etc.
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur critique lors de la suppression du fichier {Path}", path);
                return;
            }
        }
    }

    private async Task ProcessAudioAsync(Audio audio, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing Audio {AudioId}", audio.Id);
        string localPath = await storageService.DownloadAudioAsync(audio.StoragePath, cancellationToken);

        try
        {
            audio.Duration = await processingService.GetDurationAsync(localPath);
            audio.Status = MediaStatus.Ready;
            audio.PublishedAt = DateTime.UtcNow;
            audio.UpdatedAt = DateTime.UtcNow;
        }
        finally
        {
            if (File.Exists(localPath))
                File.Delete(localPath);
        }
    }
}
