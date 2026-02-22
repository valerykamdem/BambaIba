using System.Net.Sockets;
using BambaIba.Domain.Enums;

namespace BambaIba.Application.Abstractions.Interfaces;

public interface IMediaStorageService
{
    // Vidéos
    Task<string> UploadVideoAsync(Guid id, Stream stream, string fileName, string contentType, CancellationToken ct = default);
    //Task UploadMultipartAsync(string bucket, string key, Stream stream, string contentType, CancellationToken ct);
    Task<string> DownloadVideoAsync(string path, CancellationToken ct = default);

    // Audios
    Task<string> UploadAudioAsync(Guid id, Stream stream, string fileName, string contentType, CancellationToken ct = default);
    Task<string> DownloadAudioAsync(string path, CancellationToken ct = default);

    // Images (thumbnails, covers)
    Task<string> UploadImageAsync(Guid id, Stream stream, string fileName, MediaType type, CancellationToken ct = default);

    // Commun
    Task<string> GetPresignedUrlAsync(string path, string key, int expirySeconds = 3600);
    Task DeleteAsync(string path, string key);
    Task<long> GetFileSizeAsync(string path, string key, CancellationToken ct);

    //Task<string> GetVideoUrlAsync(string storagePath, bool isPublic);
    string GetPublicUrl(BucketType bucketType, string storagePath);
}

public enum MediaType
{
    Video,
    Audio,
    VideoThumbnail,
    AudioCover
}
