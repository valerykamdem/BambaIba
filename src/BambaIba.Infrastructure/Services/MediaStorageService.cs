// BambaIba.Infrastructure/Services/MinIOMediaStorageService.cs
using Amazon.S3;
using Amazon.S3.Model;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Infrastructure.Settings;
using BambaIba.SharedKernel.Videos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BambaIba.Infrastructure.Services;

public class MediaStorageService : IMediaStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly SeaweedSettings _settings;
    private readonly ILogger<MediaStorageService> _logger;
    private readonly string _tempPath;

    private const long MultipartThreshold = 200L * 1024 * 1024; // 200MB
    private const long PartSize = 50L * 1024 * 1024; // 50MB


    public MediaStorageService(
        IAmazonS3 s3Client,
        IOptions<SeaweedSettings> options,
        ILogger<MediaStorageService> logger)
    {
        _s3Client = s3Client;
        _settings = options.Value;
        _logger = logger;
        _tempPath = Path.Combine(Path.GetTempPath(), "bambaiba");
        Directory.CreateDirectory(_tempPath);
    }

    // ---------------------- UPLOADS ----------------------
    // Vidéos

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="stream"></param>
    /// <param name="fileName"></param>
    /// <param name="contentType"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<string> UploadVideoAsync(
    Guid id,
    Stream stream,
    string fileName,
    string contentType,
    CancellationToken ct = default)
    {
        string key = BuildObjectKey(id, fileName);

        await UploadAutoAsync(
            Buckets.VideoBucket,
            key,
            stream,
            contentType,
            ct);

        return key;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="stream"></param>
    /// <param name="fileName"></param>
    /// <param name="contentType"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<string> UploadAudioAsync(
     Guid id,
     Stream stream,
     string fileName,
     string contentType,
     CancellationToken ct = default)
    {
        string key = BuildObjectKey(id, fileName);

        await UploadAutoAsync(
            Buckets.AudioBucket,
            key,
            stream,
            contentType,
            ct);

        return key;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="stream"></param>
    /// <param name="fileName"></param>
    /// <param name="type"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<string> UploadImageAsync(
    Guid id,
    Stream stream,
    string fileName,
    MediaType type,
    CancellationToken ct = default)
    {
        string key = BuildObjectKey(id, fileName);

        await UploadAutoAsync(
            Buckets.ImageBucket,
            key,
            stream,
            "image/jpeg",
            ct);

        return key;
    }


    private static string BuildObjectKey(Guid id, string fileName)
    {
        return $"{id}/{fileName}";
    }

    private async Task UploadAutoAsync(
    string bucket,
    string key,
    Stream stream,
    string contentType,
    CancellationToken ct)
    {
        if (stream.CanSeek)
            stream.Position = 0;

        if (stream.Length >= MultipartThreshold)
        {
            await UploadMultipartAsync(
                bucket, key, stream, contentType, ct);
        }
        else
        {
            await UploadSimpleAsync(
                bucket, key, stream, contentType, ct);
        }
    }

    private async Task UploadSimpleAsync(
     string bucket,
     string key,
     Stream stream,
     string contentType,
     CancellationToken ct)
    {
        _logger.LogInformation("Simple upload {Bucket}/{Key}", bucket, key);

        await _s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = bucket,
            Key = key,
            InputStream = stream,
            ContentType = contentType ?? "application/octet-stream"
        }, ct);
    }

    private async Task UploadMultipartAsync(
    string bucket,
    string key,
    Stream stream,
    string contentType,
    CancellationToken ct)
    {
        _logger.LogInformation("Multipart upload {Bucket}/{Key}", bucket, key);

        InitiateMultipartUploadResponse init = await _s3Client.InitiateMultipartUploadAsync(
            new InitiateMultipartUploadRequest
            {
                BucketName = bucket,
                Key = key,
                ContentType = contentType
            }, ct);

        string uploadId = init.UploadId;

        var parts = new List<PartETag>();

        try
        {
            long position = 0;
            int partNumber = 1;

            while (position < stream.Length)
            {
                var request = new UploadPartRequest
                {
                    BucketName = bucket,
                    Key = key,
                    UploadId = uploadId,
                    PartNumber = partNumber,
                    PartSize = PartSize,
                    InputStream = stream,
                    FilePosition = position
                };

                UploadPartResponse response =
                    await _s3Client.UploadPartAsync(request, ct);

                parts.Add(new PartETag(
                    partNumber,
                    response.ETag));

                position += PartSize;
                partNumber++;
            }

            var complete = new CompleteMultipartUploadRequest
            {
                BucketName = bucket,
                Key = key,
                UploadId = uploadId
            };

            complete.AddPartETags(parts);

            await _s3Client.CompleteMultipartUploadAsync(
                complete, ct);
        }
        catch
        {
            await _s3Client.AbortMultipartUploadAsync(
                new AbortMultipartUploadRequest
                {
                    BucketName = bucket,
                    Key = key,
                    UploadId = uploadId
                }, ct);

            throw;
        }
    }


    // ---------------------- DOWNLOAD ----------------------
    public async Task<string> DownloadVideoAsync(string path, CancellationToken ct = default)
    {
        return await DownloadFromBucketAsync(Buckets.VideoBucket, path, ct);
    }

    public async Task<string> DownloadAudioAsync(string path, CancellationToken ct = default)
    {
        return await DownloadFromBucketAsync(Buckets.AudioBucket, path, ct);
    }

    private async Task<string> DownloadFromBucketAsync(
        string bucket,
        string objectName,
        CancellationToken ct)
    {
        string localPath = Path.Combine(_tempPath, $"{Guid.CreateVersion7()}{Path.GetExtension(objectName)}");

        GetObjectResponse response = await _s3Client.GetObjectAsync(bucket, objectName, ct);
        await using (FileStream fs = File.Create(localPath))
        { await response.ResponseStream.CopyToAsync(fs, ct); }
        return localPath;
    }

    // ---------------------- URLS ----------------------

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bucket"></param>
    /// <param name="key"></param>
    /// <param name="expirySeconds"></param>
    /// <returns></returns>
    public async Task<string> GetPresignedUrlAsync(
     string bucket,
     string key,
     int expirySeconds = 3600)
    {
        string url = _s3Client.GetPreSignedURL(
            new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = key,
                Expires = DateTime.UtcNow.AddSeconds(expirySeconds)
            });

        return url.Replace(
            _settings.Endpoint,
            _settings.PublicEndpoint);
    }

    public string GetPublicUrl(Domain.Enums.BucketType bucketType, string storagePath)
    {
        string bucketName = bucketType switch
        {
            Domain.Enums.BucketType.Video => Buckets.VideoBucket,
            Domain.Enums.BucketType.Audio => Buckets.AudioBucket,
            Domain.Enums.BucketType.Image => Buckets.ImageBucket,
            _ => throw new ArgumentOutOfRangeException(nameof(bucketType))
        };
        return $"{_settings.PublicEndpoint}/{bucketName}/{storagePath}";
    }

    // ---------------------- DELETE ----------------------
    public async Task DeleteAsync(string bucket,string key)
    {
        await _s3Client.DeleteObjectAsync(bucket, key);

        _logger.LogInformation(
            "Deleted {Bucket}/{Key}", bucket, key);
    }


    // ---------------------- SIZE ----------------------
    public async Task<long> GetFileSizeAsync(string bucket, string key, CancellationToken ct)
    {
        GetObjectMetadataResponse meta =
            await _s3Client.GetObjectMetadataAsync(
                bucket, key, ct);

        return meta.ContentLength;
    }

}
