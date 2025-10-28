
using Microsoft.AspNetCore.Http;

namespace BambaIba.Application.Common.Interfaces;
public interface IVideoStorageService
{
    Task<string> UploadVideoAsync(Guid videoId, IFormFile file,
        CancellationToken cancellation = default);
    Task<string> UploadFileAsync(string fileName, Stream stream, string contentType);
    Task DeleteVideoAsync(string path);
    Task<long> GetFileSizeAsync(string path);
    Task<string> GetPresignedUrlAsync(string path, int expiryInSeconds = 3600);
    Task DownloadObjectAsync(string objectName, string localFilePath);
}
