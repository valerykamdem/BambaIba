namespace BambaIba.Application.Abstractions.Interfaces;

public interface IMediaProcessingService
{
    // Commun
    Task<TimeSpan> GetDurationAsync(string localPath);

    // Vidéo spécifique
    Task<string> GenerateThumbnailAsync(Guid videoId, string localVideoPath);
    Task<string> TranscodeVideoAsync(Guid videoId, string inputPath, string quality, CancellationToken cancellationToken = default);

    // Audio spécifique
    Task<Dictionary<string, string>> GetAudioMetadataAsync(string localPath);
}
