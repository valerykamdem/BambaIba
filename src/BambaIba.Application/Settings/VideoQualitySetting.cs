namespace BambaIba.Application.Settings;
public static class VideoQualitySetting
{
    // Liste ordonnée des labels disponibles
    public static readonly string[] All =
    [
        "240p", "360p", "480p", /*"720p", "1080p"*/
    ];

    // Dictionnaire centralisé des configs
    public static readonly Dictionary<string, VideoQualityConfig> Configs =
        new()
        {
            ["240p"] = new VideoQualityConfig { Width = 426, Height = 240, Bitrate = "400k" },
            ["360p"] = new VideoQualityConfig { Width = 640, Height = 360, Bitrate = "800k" },
            ["480p"] = new VideoQualityConfig { Width = 854, Height = 480, Bitrate = "1200k" },
            //["720p"] = new VideoQualityConfig { Width = 1280, Height = 720, Bitrate = "2500k" },
            //["1080p"] = new VideoQualityConfig { Width = 1920, Height = 1080, Bitrate = "5000k" }
        };

    // Helper pour récupérer une config
    public static VideoQualityConfig Get(string quality)
    {
        if (Configs.TryGetValue(quality, out VideoQualityConfig? config))
            return config;

        throw new ArgumentException($"Qualité inconnue : {quality}");
    }
}

public class VideoQualityConfig
{
    public int Width { get; set; }
    public int Height { get; set; }
    public string Bitrate { get; set; } = string.Empty;
}
