// BambaIba.Infrastructure/Services/FFmpegAudioProcessingService.cs
using System.Diagnostics;
using System.Text.RegularExpressions;
using BambaIba.Domain.Audios;
using BambaIba.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BambaIba.Infrastructure.Services;

public class FFmpegAudioProcessingService : IAudioProcessingService
{
    private readonly FFmpegSettings _settings;
    private readonly ILogger<FFmpegAudioProcessingService> _logger;

    public FFmpegAudioProcessingService(
        IOptions<FFmpegSettings> options,
        ILogger<FFmpegAudioProcessingService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<TimeSpan> GetAudioDurationAsync(string localAudioPath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _settings.ExecutablePath,
                Arguments = $"-i \"{localAudioPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string output = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        Match match = Regex.Match(output, @"Duration: (\d{2}):(\d{2}):(\d{2})\.(\d{2})");

        if (match.Success)
        {
            int hours = int.Parse(match.Groups[1].Value);
            int minutes = int.Parse(match.Groups[2].Value);
            int seconds = int.Parse(match.Groups[3].Value);
            int milliseconds = int.Parse(match.Groups[4].Value) * 10;

            return new TimeSpan(0, hours, minutes, seconds, milliseconds);
        }

        return TimeSpan.Zero;
    }
}
