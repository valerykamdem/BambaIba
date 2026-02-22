
namespace BambaIba.Infrastructure.Settings;

public sealed class SeaweedSettings
{
    public const string SectionName = "SeaweedFS";

    public string Endpoint { get; set; } = default!; 
    public string PublicEndpoint { get; set; } = default!; 
    public string AccessKey { get; set; } = default!; 
    public string SecretKey { get; set; } = default!;
    public BucketNames Buckets { get; set; } = new();
}

public sealed class BucketNames 
{ 
    public string Video { get; set; } = "bambaiba-videos"; 
    public string Audio { get; set; } = "bambaiba-audios"; 
    public string Image { get; set; } = "bambaiba-images"; 
}
