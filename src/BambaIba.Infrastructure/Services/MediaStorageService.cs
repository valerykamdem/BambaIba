// BambaIba.Infrastructure/Services/MinIOMediaStorageService.cs
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Enums;
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

    public async Task<string> UploadVideoStreamAsync(
        Guid mediaId,
        Stream stream,
        string key,
        string contentType,
        CancellationToken ct)
    {
        var transfer = new TransferUtility(_s3Client);

        var request = new TransferUtilityUploadRequest
        {
            BucketName = Buckets.VideoBucket,
            Key = key,
            InputStream = stream,
            ContentType = contentType,
            AutoCloseStream = false
        };

        await transfer.UploadAsync(request, ct);

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
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        // Si seekable → multipart possible
        if (stream.CanSeek)
        {
            stream.Position = 0;

            if (stream.Length >= MultipartThreshold)
            {
                await UploadMultipartAsync(bucket, key, stream, contentType, ct);
                return;
            }

            await UploadSimpleAsync(bucket, key, stream, contentType, ct);
            return;
        }

        // Sinon : stream non seekable → TransferUtility obligatoire
        await UploadSimpleAsync(bucket, key, stream, contentType, ct);
    }


    private async Task UploadSimpleAsync(
    string bucket,
    string key,
    Stream stream,
    string contentType,
    CancellationToken ct)
    {
        var transfer = new TransferUtility(_s3Client);

        var request = new TransferUtilityUploadRequest
        {
            BucketName = bucket,
            Key = key,
            InputStream = stream,
            ContentType = contentType,
            AutoCloseStream = false
        };

        await transfer.UploadAsync(request, ct);
    }


    private async Task UploadMultipartAsync(
     string bucket,
     string key,
     Stream stream,
     string contentType,
     CancellationToken ct)
    {
        InitiateMultipartUploadResponse initiate = await _s3Client.InitiateMultipartUploadAsync(
            new InitiateMultipartUploadRequest
            {
                BucketName = bucket,
                Key = key,
                ContentType = contentType
            },
            ct);

        string uploadId = initiate.UploadId;
        var partETags = new List<PartETag>();

        long partSize = 5 * 1024 * 1024; // 5MB
        long fileSize = stream.Length;
        long position = 0;
        int partNumber = 1;

        while (position < fileSize)
        {
            long size = Math.Min(partSize, fileSize - position);

            var request = new UploadPartRequest
            {
                BucketName = bucket,
                Key = key,
                UploadId = uploadId,
                PartNumber = partNumber,
                InputStream = stream,
                PartSize = size
            };

            UploadPartResponse response = await _s3Client.UploadPartAsync(request, ct);
            partETags.Add(new PartETag(partNumber, response.ETag));

            position += size;
            partNumber++;
        }

        await _s3Client.CompleteMultipartUploadAsync(
            new CompleteMultipartUploadRequest
            {
                BucketName = bucket,
                Key = key,
                UploadId = uploadId,
                PartETags = partETags
            },
            ct);
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
    public async Task DeleteAsync(string bucket, string key)
    {
        await _s3Client.DeleteObjectAsync(bucket, key);

        _logger.LogInformation(
            "Deleted {Bucket}/{Key}", bucket, key);
    }


    // ---------------------- SIZE ----------------------

    public async Task<long> GetFileSizeAsync(string key, CancellationToken ct)
    {
        try
        {
            GetObjectMetadataResponse meta = await _s3Client.GetObjectMetadataAsync(Buckets.VideoBucket, key, ct);
            return meta.ContentLength;
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"[DEBUG S3] ERREUR: {ex.Message}");
            throw;
        }
    }

}
