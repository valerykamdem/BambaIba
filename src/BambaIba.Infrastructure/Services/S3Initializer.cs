using Amazon.S3;
using Amazon.S3.Model;
using BambaIba.SharedKernel.Videos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BambaIba.Infrastructure.Services;

public class S3Initializer : IHostedService
{
    private readonly IAmazonS3 _s3;
    private readonly ILogger<S3Initializer> _logger;

    public S3Initializer(
        IAmazonS3 s3,
        ILogger<S3Initializer> logger)
    {
        _s3 = s3;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        string[] buckets =
        {
            Buckets.VideoBucket,
            Buckets.AudioBucket,
            Buckets.ImageBucket
        };

        ListBucketsResponse existing = await _s3.ListBucketsAsync(ct);

        foreach (string bucket in buckets)
        {
            if (!existing.Buckets.Any(b => b.BucketName == bucket))
            {
                _logger.LogInformation("Creating bucket {B}", bucket);

                await _s3.PutBucketAsync(bucket, ct);
            }
        }

        _logger.LogInformation("S3 initialization done");
    }

    public Task StopAsync(CancellationToken ct)
        => Task.CompletedTask;
}
