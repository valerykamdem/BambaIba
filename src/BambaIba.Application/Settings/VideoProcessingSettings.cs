// BambaIba.Application/Settings/VideoProcessingSettings.cs
namespace BambaIba.Application.Settings;

public class VideoProcessingSettings
{
    public const string SectionName = "VideoProcessing";

    public long MaxFileSizeBytes { get; set; } = 5_368_709_120; // 5GB
    public int MaxDurationSeconds { get; set; } = 3600; // 1 heure
    public List<string> AllowedMimeTypes { get; set; } = new()
    {
        "video/mp4",
        "video/mpeg",
        "video/quicktime",
        "video/x-msvideo",
        "video/webm"
    };
    public List<string> AllowedExtensions { get; set; } = new()
    {
        ".mp4", ".mpeg", ".mov", ".avi", ".webm"
    };
    public bool AutoGenerateThumbnail { get; set; } = true;
    public bool AutoTranscode { get; set; } = true;
}
