using BambaIba.Infrastructure.Settings;

namespace BambaIba.Api.Helpers;

public static class FFmpegConfigHelper
{
    public static void Normalize(FFmpegSettings settings)
    {
        if (OperatingSystem.IsWindows())
        {
            // Sur Windows, on laisse "ffmpeg" (trouvé via PATH)
            settings.ExecutablePath = "ffmpeg";
            settings.TempDirectory = Path.Combine(Path.GetTempPath(), "bambaiba");
        }
        else
        {
            // Sur Linux/Docker
            if (string.IsNullOrWhiteSpace(settings.ExecutablePath))
                settings.ExecutablePath = "/usr/bin/ffmpeg";

            if (string.IsNullOrWhiteSpace(settings.TempDirectory))
                settings.TempDirectory = "/tmp/bambaiba";
        }
    }
}
