// BambaIba.Infrastructure/Settings/MinIOSettings.cs
namespace BambaIba.Infrastructure.Settings;

public class MinIOSettings
{
    public const string SectionName = "MinIO";

    public string Endpoint { get; set; } = string.Empty;
    public string PublicEndpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public bool UseSSL { get; set; } = false;
    public string Region { get; set; } = "us-east-1";

    // Validation
    public bool IsValid() =>
        !string.IsNullOrEmpty(Endpoint) &&
        !string.IsNullOrEmpty(AccessKey) &&
        !string.IsNullOrEmpty(BucketName);
}
