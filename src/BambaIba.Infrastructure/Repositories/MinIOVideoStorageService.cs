// BambaIba.Infrastructure/Services/MinIOVideoStorageService.cs
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Videos;
using BambaIba.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel.Args;


namespace BambaIba.Infrastructure.Repositories;

public class MinIOVideoStorageService : IVideoStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinIOVideoStorageService> _logger;
    private readonly MinIOSettings _settings;

    public MinIOVideoStorageService(IMinioClient minioClient,
        IOptions<MinIOSettings> options,
        ILogger<MinIOVideoStorageService> logger)
    {
        _minioClient = minioClient;
        _settings = options.Value;
        _logger = logger;
        InitializeBucketAsync().GetAwaiter().GetResult();
    }

    private async Task InitializeBucketAsync()
    {
        bool bucketExists = await _minioClient.BucketExistsAsync(
        new BucketExistsArgs().WithBucket(_settings.BucketName));

        if (!bucketExists)
        {
            _logger.LogInformation("Creating bucket {BucketName}", _settings.BucketName);

            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_settings.BucketName));

            _logger.LogInformation("Bucket {BucketName} created", _settings.BucketName);
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
                    "Resource": ["arn:aws:s3:::{{_settings.BucketName}}/*"]
                }
            ]
        }
        """;

        await _minioClient.SetPolicyAsync(
            new SetPolicyArgs()
                .WithBucket(_settings.BucketName)
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
                    .WithBucket(_settings.BucketName)
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
                    .WithBucket(_settings.BucketName)
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
                _settings.BucketName);
            throw;
        }
    }

    public async Task<string> UploadFileAsync(string fileName, Stream stream, string contentType)
    {
        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(fileName)
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
                .WithBucket(_settings.BucketName)
                .WithPrefix(prefix)
                .WithRecursive(true));

            await foreach (Minio.DataModel.Item? item in observable)
            {
                objectsToDelete.Add(item.Key);
            }

            foreach (string objKey in objectsToDelete)
            {
                await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                    .WithBucket(_settings.BucketName)
                    .WithObject(objKey));
            }

            _logger.LogInformation("Supprimé {Count} objets sous le préfixe {Prefix}", objectsToDelete.Count, prefix);
        }

        _logger.LogInformation("✅ Tous les fichiers liés à la vidéo {VideoId} ont été supprimés", videoId);
    }

    public async Task<long> GetFileSizeAsync(string storagePath)
    {
        Minio.DataModel.ObjectStat stat = await _minioClient.StatObjectAsync(new StatObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(storagePath));

        return stat.Size;
    }

    public async Task<string> GetPresignedUrlAsync(string storagePath, int expiryInSeconds = 3600)
    {
        string url = await _minioClient.PresignedGetObjectAsync(
         new PresignedGetObjectArgs()
             .WithBucket(_settings.BucketName)
             .WithObject(storagePath)
             .WithExpiry(expiryInSeconds));

        // ✅ Remplacer minio:9000 par localhost:9000
        return url.Replace(_settings.Endpoint, _settings.PublicEndpoint);
    }

    public async Task DownloadObjectAsync(string objectName, string localFilestoragePath)
    {
        GetObjectArgs args = new GetObjectArgs()
            .WithBucket(_settings.BucketName)
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
        return $"http://{_settings.PublicEndpoint}/{_settings.BucketName}/{storagePath}";
    }
}
