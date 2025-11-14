
namespace BambaIba.Domain.Audios;
public interface IAudioStorageService
{
    Task<string> UploadAudioAsync(
        Guid audioId,
        Stream audioStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<string> UploadCoverImageAsync(
        Guid audioId,
        Stream imageStream,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<string> DownloadAudioAsync(
        string storagePath,
        CancellationToken cancellationToken = default);

    Task<string> GetPresignedUrlAsync(string storagePath, int expiryInSeconds = 3600);

    Task DeleteAudioAsync(string storagePath);
}
