namespace BambaIba.SharedKernel.Videos;
public record VideoQualityDto
{
    public string Quality { get; init; } = string.Empty;
    //public string StoragePath { get; init; } = string.Empty;
    public string PublicUrl {get; init; } = string.Empty;
}
