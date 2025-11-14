// BambaIba.Infrastructure/Services/MinIOMediaStorageService.cs
using System.Net.Sockets;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Enums;
using BambaIba.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace BambaIba.Infrastructure.Services;

public class MinIOMediaStorageService : IMediaStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinIOSettings _settings;
    private readonly ILogger<MinIOMediaStorageService> _logger;
    private readonly string _tempPath;

    // Noms des buckets
    private const string VideoBucket = "bambaiba-videos";
    private const string AudioBucket = "bambaiba-audios";
    private const string ImageBucket = "bambaiba-images";

    public MinIOMediaStorageService(
        IMinioClient minioClient,
        IOptions<MinIOSettings> options,
        ILogger<MinIOMediaStorageService> logger)
    {
        _minioClient = minioClient;
        _settings = options.Value;
        _logger = logger;
        _tempPath = Path.Combine(Path.GetTempPath(), "bambaiba");
        Directory.CreateDirectory(_tempPath);

        InitializeBucketsAsync().GetAwaiter().GetResult();
    }

    private async Task InitializeBucketsAsync()
    {
        string[] buckets = [VideoBucket, AudioBucket, ImageBucket];

        foreach (string bucket in buckets)
        {
            bool exists = await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(bucket));

            if (!exists)
            {
                await _minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(bucket));

                await SetPublicPolicyAsync(bucket);

                _logger.LogInformation("Bucket created and set to public: {Bucket}", bucket);
            }
        }
    }

    private async Task SetPublicPolicyAsync(string bucket)
    {
        string policy = $$"""
        {
            "Version": "2012-10-17",
            "Statement": [
                {
                    "Effect": "Allow",
                    "Principal": {"AWS": ["*"]},
                    "Action": ["s3:GetObject"],
                    "Resource": ["arn:aws:s3:::{{bucket}}/*"]
                }
            ]
        }
        """;

        await _minioClient.SetPolicyAsync(
            new SetPolicyArgs()
                .WithBucket(bucket)
                .WithPolicy(policy));
    }

    // Vidéos
    public async Task<string> UploadVideoAsync(
        Guid id,
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken ct = default)
    {
        string objectName = $"videos/{id}/{fileName}";
        await UploadToBucketAsync(VideoBucket, objectName, stream, contentType, ct);
        return objectName;
    }

    public async Task<string> DownloadVideoAsync(string path, CancellationToken ct = default)
    {
        return await DownloadFromBucketAsync(VideoBucket, path, ct);
    }

    // Audios
    public async Task<string> UploadAudioAsync(
        Guid id,
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken ct = default)
    {
        string objectName = $"audios/{id}/{fileName}";
        await UploadToBucketAsync(AudioBucket, objectName, stream, contentType, ct);
        return objectName;
    }

    public async Task<string> DownloadAudioAsync(string path, CancellationToken ct = default)
    {
        return await DownloadFromBucketAsync(AudioBucket, path, ct);
    }

    // Images
    public async Task<string> UploadImageAsync(
        Guid id,
        Stream stream,
        string fileName,
        MediaType type,
        CancellationToken ct = default)
    {
        string folder = type switch
        {
            MediaType.VideoThumbnail => $"thumbnails/videos/{id}",
            MediaType.AudioCover => $"covers/audios/{id}",
            _ => $"images/{id}"
        };

        string objectName = $"{folder}/{fileName}";

        await UploadToBucketAsync(ImageBucket, objectName, stream, "image/jpeg", ct);
        return objectName;
    }

    // Méthodes privées communes
    private async Task UploadToBucketAsync(
        string bucket,
        string objectName,
        Stream stream,
        string contentType,
        CancellationToken ct)
    {
        _logger.LogInformation("Uploading to {Bucket}/{Object}", bucket, objectName);

        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType), ct);
    }

    private async Task<string> DownloadFromBucketAsync(
        string bucket,
        string objectName,
        CancellationToken ct)
    {
        string localPath = Path.Combine(_tempPath, $"{Guid.CreateVersion7()}{Path.GetExtension(objectName)}");

        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName)
            .WithCallbackStream(stream =>
            {
                using FileStream fileStream = File.Create(localPath);
                stream.CopyTo(fileStream);
            }), ct);

        return localPath;
    }

    public async Task<string> GetPresignedUrlAsync(string path, int expirySeconds = 3600)
    {
        // Déterminer le bucket depuis le path
        string bucket = path.StartsWith("videos/") ? VideoBucket :
                     path.StartsWith("audios/") ? AudioBucket :
                     ImageBucket;

        string url = await _minioClient.PresignedGetObjectAsync(
            new PresignedGetObjectArgs()
                .WithBucket(bucket)
                .WithObject(path)
                .WithExpiry(expirySeconds));

        return url.Replace(_settings.Endpoint, _settings.PublicEndpoint);
    }

    public async Task DeleteAsync(string path)
    {
        string bucket = path.StartsWith("videos/") ? VideoBucket :
                     path.StartsWith("audios/") ? AudioBucket :
                     ImageBucket;

        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(bucket)
            .WithObject(path));
    }

    public async Task<long> GetFileSizeAsync(string path)
    {
        string bucket = path.StartsWith("videos/") ? VideoBucket :
                     path.StartsWith("audios/") ? AudioBucket :
                     ImageBucket;

        Minio.DataModel.ObjectStat stat = await _minioClient.StatObjectAsync(new StatObjectArgs()
            .WithBucket(bucket)
            .WithObject(path));

        return stat.Size;
    }

    public async Task<string> GetVideoUrlAsync(string storagePath, bool isPublic)
    {
        if (isPublic)
            return GetPublicUrl(BucketType.Video, storagePath);

        return await GetPresignedUrlAsync(storagePath);
    }

    public string GetPublicUrl(BucketType bucketType, string storagePath)
    {
        string bucketName = bucketType switch
        {
            BucketType.Video => _settings.Buckets.Video,
            BucketType.Audio => _settings.Buckets.Audio,
            BucketType.Image => _settings.Buckets.Image,
            _ => throw new ArgumentOutOfRangeException(nameof(bucketType), bucketType, null)
        };

        return $"{_settings.PublicEndpoint}/{bucketName}/{storagePath}";
    }
}
