// BambaIba.Infrastructure/Settings/FFmpegSettings.cs
namespace BambaIba.Infrastructure.Settings;

public class FFmpegSettings
{
    public const string SectionName = "FFmpeg";

    public string ExecutablePath { get; set; } = "/usr/bin/ffmpeg";
    public string TempDirectory { get; set; } = "/tmp/bambaiba";
    public int MaxConcurrentJobs { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 3600;

    public Dictionary<string, VideoQualityConfig> Qualities { get; set; } = new()
    {
        ["360p"] = new() { Width = 640, Height = 360, Bitrate = "800k" },
        ["480p"] = new() { Width = 854, Height = 480, Bitrate = "1200k" },
        ["720p"] = new() { Width = 1280, Height = 720, Bitrate = "2500k" },
        ["1080p"] = new() { Width = 1920, Height = 1080, Bitrate = "5000k" }
    };
}

public class VideoQualityConfig
{
    public int Width { get; set; }
    public int Height { get; set; }
    public string Bitrate { get; set; } = string.Empty;
}
