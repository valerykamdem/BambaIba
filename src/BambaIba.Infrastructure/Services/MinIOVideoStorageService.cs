// BambaIba.Infrastructure/Services/MinIOVideoStorageService.cs
using BambaIba.Domain.Audios;
using BambaIba.Domain.Videos;
using BambaIba.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel.Args;


namespace BambaIba.Infrastructure.Services;

public class MinIOVideoStorageService : IVideoStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinIOVideoStorageService> _logger;
    private readonly MinIOSettings _settings;
    //private readonly string _tempPath;

    public MinIOVideoStorageService(IMinioClient minioClient,
        IOptions<MinIOSettings> options,
        ILogger<MinIOVideoStorageService> logger)
    {
        _minioClient = minioClient;
        _settings = options.Value;
        _logger = logger;
        //_tempPath = Path.Combine(Path.GetTempPath(), "bambaiba-video");
        //Directory.CreateDirectory(_tempPath);
        InitializeBucketAsync().GetAwaiter().GetResult();
    }

    private async Task InitializeBucketAsync()
    {
        bool bucketExists = await _minioClient.BucketExistsAsync(
        new BucketExistsArgs().WithBucket(_settings.Buckets.Video));

        if (!bucketExists)
        {
            _logger.LogInformation("Creating bucket {BucketName}", _settings.Buckets.Video);

            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_settings.Buckets.Video));

            _logger.LogInformation("Bucket {BucketName} created", _settings.Buckets.Video);
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
                    "Resource": ["arn:aws:s3:::{{_settings.Buckets.Video}}/*"]
                }
            ]
        }
        """;

        await _minioClient.SetPolicyAsync(
            new SetPolicyArgs()
                .WithBucket(_settings.Buckets.Video)
                .WithBucket(_settings.Buckets.Audio)
                .WithBucket(_settings.Buckets.Image)
                .WithPolicy(policy));
    }

    public async Task<string> UploadVideoAsync(Guid videoId, IFormFile file,
        CancellationToken cancellation)
    {

        try
        {

            // 2. Créer le nom du fichier
            string fileName = $"videos/{videoId}/{file.FileName}";

            // 1. Définir un chemin temporaire
            string tempFilePath = Path.GetTempFileName();

            // 2. Copier le contenu de IFormFile sur le disque
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream, cancellation);
            }

            // 3. Ouvrir le FileStream pour l'upload (il est seekable)
            using (var uploadStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read))
            {
                await _minioClient.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(_settings.Buckets.Video)
                    .WithObject(fileName)
                    .WithStreamData(uploadStream)
                    .WithObjectSize(uploadStream.Length)
                    .WithContentType(file.ContentType), cancellation);
            }

            // 4. Nettoyer le fichier temporaire (très important !)
            File.Delete(tempFilePath);

            _logger.LogInformation(
                "File uploaded successfully: {FileName}, Size: {Size} bytes",
                fileName,
                file.Length);

            // 5. Vérifier que le fichier existe vraiment
            Minio.DataModel.ObjectStat statObject = await _minioClient.StatObjectAsync(
                new StatObjectArgs()
                    .WithBucket(_settings.Buckets.Video)
                    .WithObject(fileName));

            _logger.LogInformation(
                "File verified in MinIO. Size: {Size}, ETag: {ETag}",
                statObject.Size,
                statObject.ETag);

            return fileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to upload video {VideoId}. Settings: Endpoint={Endpoint}, Bucket={Bucket}",
                videoId,
                _settings.Endpoint,
                _settings.Buckets.Video);
            throw;
        }
    }

    public async Task<string> UploadFileAsync(Guid videoId,
        Stream stream,string fileName, string contentType)
    {
        //string objectName = $"videos/{videoId}/cover/{fileName}";
        string objectName = $"thumbnails/{videoId}.jpg";

        _logger.LogInformation("Uploading cover image to MinIO: {ObjectName}", objectName);

        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_settings.Buckets.Image)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType));

        return fileName;
    }

    public async Task DeleteVideoAsync(string videoId)
    {
        //await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
        //    .WithBucket(_settings.BucketName)
        //    .WithObject(storagePath));

        string[] prefixes = [
            $"videos/{videoId}/",
            $"thumbnails/{videoId}/"
        ];

        foreach (string prefix in prefixes)
        {
            var objectsToDelete = new List<string>();

            IAsyncEnumerable<Minio.DataModel.Item> observable =  _minioClient.ListObjectsEnumAsync(new ListObjectsArgs()
                .WithBucket(_settings.Buckets.Video)
                .WithPrefix(prefix)
                .WithRecursive(true));

            await foreach (Minio.DataModel.Item? item in observable)
            {
                objectsToDelete.Add(item.Key);
            }

            foreach (string objKey in objectsToDelete)
            {
                await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                    .WithBucket(_settings.Buckets.Video)
                    .WithObject(objKey));
            }

            _logger.LogInformation("Supprimé {Count} objets sous le préfixe {Prefix}", objectsToDelete.Count, prefix);
        }

        _logger.LogInformation("✅ Tous les fichiers liés à la vidéo {VideoId} ont été supprimés", videoId);
    }

    public async Task<long> GetFileSizeAsync(string storagePath)
    {
        Minio.DataModel.ObjectStat stat = await _minioClient.StatObjectAsync(new StatObjectArgs()
            .WithBucket(_settings.Buckets.Video)
            .WithObject(storagePath));

        return stat.Size;
    }

    public async Task<string> GetPresignedUrlAsync(string storagePath, int expiryInSeconds = 3600)
    {
        string url = await _minioClient.PresignedGetObjectAsync(
         new PresignedGetObjectArgs()
             .WithBucket(_settings.Buckets.Video)
             .WithObject(storagePath)
             .WithExpiry(expiryInSeconds));

        // ✅ Remplacer minio:9000 par localhost:9000
        return url.Replace(_settings.Endpoint, _settings.PublicEndpoint);
    }

    public async Task DownloadObjectAsync(string objectName, string localFilestoragePath)
    {
        GetObjectArgs args = new GetObjectArgs()
            .WithBucket(_settings.Buckets.Video)
            .WithObject(objectName)
            .WithFile(localFilestoragePath); // <--- Spécifie le chemin du fichier local

        await _minioClient.GetObjectAsync(args);
    }

    public async Task<string> GetVideoUrlAsync(string storagePath, bool isPublic)
    {
        if (isPublic)
            return GetPublicUrl(storagePath);

        return await GetPresignedUrlAsync(storagePath);
    }

    public string GetPublicUrl(string storagePath)
    {
        return $"http://{_settings.PublicEndpoint}/{_settings.Buckets.Video}/{storagePath}";
    }
}
