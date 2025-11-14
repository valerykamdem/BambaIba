using Microsoft.AspNetCore.Http;

namespace BambaIba.Domain.Videos;

public interface IVideoStorageService
{
    // Upload vidéo originale
    Task<string> UploadVideoAsync(Guid videoId, IFormFile file,
        CancellationToken cancellation = default);
    // Méthode pour obtenir URL (publique ou présignée)
    Task<string> GetVideoUrlAsync(string storagePath, bool isPublic);
    string GetPublicUrl(string storagePath);
    Task<string> UploadFileAsync(Guid videoId,
        Stream stream, string fileName, string contentType);
    Task DeleteVideoAsync(string storagePath);
    Task<long> GetFileSizeAsync(string storagePath);
    Task<string> GetPresignedUrlAsync(string storagePath, int expiryInSeconds = 3600);
    Task DownloadObjectAsync(string objectName, string localFilestoragePath);

}
