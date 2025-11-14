// BambaIba.Infrastructure/Settings/MinIOSettings.cs
namespace BambaIba.Infrastructure.Settings;

public class MinIOSettings
{
    public const string SectionName = "MinIO";

    public string Endpoint { get; set; } = string.Empty;
    public string PublicEndpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    //public string BucketName { get; set; } = string.Empty;
    public bool UseSSL { get; set; } = false;
    public string Region { get; set; } = "us-east-1";
    public BucketNames Buckets { get; set; } = new();

    // Validation
    public bool IsValid() =>
        !string.IsNullOrEmpty(Endpoint) &&
        !string.IsNullOrEmpty(AccessKey) &&
        !string.IsNullOrEmpty(Buckets.Video) &&
        !string.IsNullOrEmpty(Buckets.Audio) &&
        !string.IsNullOrEmpty(Buckets.Image);
}

public class BucketNames
{
    public string Video { get; set; } = string.Empty;
    public string Audio { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}
