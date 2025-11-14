// BambaIba.Infrastructure/Services/MinIOAudioStorageService.cs
using BambaIba.Domain.Audios;
using BambaIba.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace BambaIba.Infrastructure.Services;

public class MinIOAudioStorageService : IAudioStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinIOSettings _settings;
    private readonly ILogger<MinIOAudioStorageService> _logger;
    private readonly string _tempPath;

    public MinIOAudioStorageService(
        IMinioClient minioClient,
        IOptions<MinIOSettings> options,
        ILogger<MinIOAudioStorageService> logger)
    {
        _minioClient = minioClient;
        _settings = options.Value;
        _logger = logger;
        _tempPath = Path.Combine(Path.GetTempPath(), "bambaiba-audio");
        Directory.CreateDirectory(_tempPath);
        InitializeBucketAsync().GetAwaiter().GetResult();
    }

    private async Task InitializeBucketAsync()
    {
        bool bucketExists = await _minioClient.BucketExistsAsync(
        new BucketExistsArgs().WithBucket(_settings.Buckets.Audio));

        if (!bucketExists)
        {
            _logger.LogInformation("Creating bucket {BucketName}", _settings.Buckets.Audio);

            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_settings.Buckets.Audio));

            _logger.LogInformation("Bucket {BucketName} created", _settings.Buckets.Audio);
        }
        // ✅ Rendre le bucket public (lecture seulement)
        string policy = $$"""
        {
            "Version": "2012-10-17",
            "Statement": [
                {
                    "Effect": "Allow",
                    "Principal": {"AWS": ["*"]},
                    "Action": ["s3:GetObject"],
                    "Resource": ["arn:aws:s3:::{{_settings.Buckets.Audio}}/*"]
                }
            ]
        }
        """;

        await _minioClient.SetPolicyAsync(
            new SetPolicyArgs()
                .WithBucket(_settings.Buckets.Audio)
                .WithPolicy(policy));
    }

    public async Task<string> UploadAudioAsync(
        Guid audioId,
        Stream audioStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        string objectName = $"audios/{audioId}/audio/{fileName}";

        _logger.LogInformation("Uploading audio to MinIO: {ObjectName}", objectName);

        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_settings.Buckets.Audio)
            .WithObject(objectName)
            .WithStreamData(audioStream)
            .WithObjectSize(audioStream.Length)
            .WithContentType(contentType), cancellationToken);

        return objectName;
    }

    public async Task<string> UploadCoverImageAsync(
        Guid audioId,
        Stream imageStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        string objectName = $"audios/{audioId}/cover/{fileName}";

        _logger.LogInformation("Uploading cover image to MinIO: {ObjectName}", objectName);

        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_settings.Buckets.Image)
            .WithObject(objectName)
            .WithStreamData(imageStream)
            .WithObjectSize(imageStream.Length)
            .WithContentType("image/jpeg"), cancellationToken);

        return objectName;
    }

    public async Task<string> DownloadAudioAsync(
        string storagePath,
        CancellationToken cancellationToken = default)
    {
        string localPath = Path.Combine(_tempPath, $"{Guid.NewGuid()}{Path.GetExtension(storagePath)}");

        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_settings.Buckets.Audio)
            .WithObject(storagePath)
            .WithCallbackStream(stream =>
            {
                using FileStream fileStream = File.Create(localPath);
                stream.CopyTo(fileStream);
            }), cancellationToken);

        return localPath;
    }

    public async Task<string> GetPresignedUrlAsync(string storagePath, int expiryInSeconds = 3600)
    {
        string url = await _minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(_settings.Buckets.Audio)
            .WithObject(storagePath)
            .WithExpiry(expiryInSeconds));

        return url.Replace(_settings.Endpoint, _settings.PublicEndpoint);
    }

    public async Task DeleteAudioAsync(string storagePath)
    {
        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_settings.Buckets.Audio)
            .WithObject(storagePath));
    }
}
