using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Features.Search;
using BambaIba.Application.Settings;
using BambaIba.Domain.Entities.Audios;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.VideoQualities;
using BambaIba.Domain.Entities.Videos;
using BambaIba.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BambaIba.Application.Features.MediaBase.ProcessMedia;

public sealed record ProcessMediaCommand(Guid MediaId);

public sealed class ProcessMediaHandler(
        IBIDbContext dbContext,
        IMediaStorageService storageService,
        IMediaProcessingService processingService,
        IMessageBus bus,
        ILogger<ProcessMediaHandler> logger)
{

    public async Task Handle(
    ProcessMediaCommand command,
    CancellationToken cancellationToken)
    {
        MediaAsset? media =
            await dbContext.MediaAssets
                .FirstOrDefaultAsync(m => m.Id == command.MediaId, cancellationToken: cancellationToken);

        if (media == null)
            return;

        try
        {
            // Phase 1
            media.Status = MediaStatus.Processing;
            await dbContext.SaveChangesAsync(CancellationToken.None);

            // Phase 2
            if (media is Video v)
                await ProcessVideoAsync(v, cancellationToken);

            else if (media is Audio a)
                await ProcessAudioAsync(a, cancellationToken);

            // Phase 3
            media.Status = MediaStatus.Ready;
            media.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(CancellationToken.None);

            // on lance l'indexation !
            await bus.PublishAsync(new IndexMediaCommand(media.Id));

            logger.LogInformation("Media {Id} done", media.Id);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Processing canceled {Id}", command.MediaId);

            media.Status = MediaStatus.Pending;
            await dbContext.SaveChangesAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Processing failed {Id}", command.MediaId);

            media.Status = MediaStatus.Failed;
            await dbContext.SaveChangesAsync(CancellationToken.None);
        }
    }


    private async Task ProcessVideoAsync(Video video, CancellationToken ct)
    {
        string localPath =
            await storageService.DownloadVideoAsync(video.StoragePath, ct);

        try
        {
            video.Duration =
                await processingService.GetDurationAsync(localPath);

            if (string.IsNullOrEmpty(video.ThumbnailPath))
            {
                video.ThumbnailPath =
                    await processingService.GenerateThumbnailAsync(video.Id, localPath);
            }

            foreach (string quality in VideoQualitySetting.All)
            {
                string path =
                    await processingService.TranscodeVideoAsync(
                        video.Id,
                        video.StoragePath,
                        quality,
                        ct);

                long size =
                    await storageService.GetFileSizeAsync(path, video.Topic, ct);

                dbContext.VideoQualities.Add(new VideoQuality
                {
                    Id = Guid.CreateVersion7(),
                    MediaId = video.Id,
                    Quality = quality,
                    StoragePath = path,
                    FileSize = size,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
        finally
        {
            if (File.Exists(localPath))
                File.Delete(localPath);
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
