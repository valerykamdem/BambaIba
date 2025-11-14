namespace BambaIba.SharedKernel.Videos;
public record VideoQualityDto
{
    public string Quality { get; init; } = string.Empty;
    //public string StoragePath { get; init; } = string.Empty;
    public string VideoUrl { get; init; } = string.Empty;
}
