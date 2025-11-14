// BambaIba.Infrastructure/Services/FFmpegMediaProcessingService.cs
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Settings;
using BambaIba.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BambaIba.Infrastructure.Services;

public class FFmpegMediaProcessingService : IMediaProcessingService
{
    private readonly FFmpegSettings _settings;
    private readonly ILogger<FFmpegMediaProcessingService> _logger;
    private readonly IMediaStorageService _storageService;
    private readonly string _tempPath;

    public FFmpegMediaProcessingService(
        IOptions<FFmpegSettings> options,
        IMediaStorageService storageService,
        ILogger<FFmpegMediaProcessingService> logger)
    {
        _settings = options.Value;
        _storageService = storageService;
        _logger = logger;
        _tempPath = Path.Combine(Path.GetTempPath(), "bambaiba");
        Directory.CreateDirectory(_tempPath);
    }

    // Commun : Durée
    public async Task<TimeSpan> GetDurationAsync(string localPath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _settings.ExecutablePath,
                Arguments = $"-i \"{localPath}\"",
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

    // Vidéo : Thumbnail
    public async Task<string> GenerateThumbnailAsync(Guid videoId, string localVideoPath)
    {
        string thumbnailPath = Path.Combine(_tempPath, $"{videoId}_thumb.jpg");

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _settings.ExecutablePath,
                Arguments = $"-i \"{localVideoPath}\" -ss 00:00:01.000 -vframes 1 \"{thumbnailPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        string output = await outputTask;
        string error = await errorTask;

        if (!string.IsNullOrWhiteSpace(error))
        {
            _logger.LogWarning("FFmpeg Thumbnail error: {Error}", error);
        }

        if (File.Exists(thumbnailPath))
        {
            using FileStream stream = File.OpenRead(thumbnailPath);
            string storagePath = $"{videoId}.jpg";
            await _storageService.UploadImageAsync(videoId, stream, storagePath, MediaType.VideoThumbnail);

            CleanupFile(localVideoPath);
            CleanupFile(thumbnailPath);

            return storagePath;
        }

        CleanupFile(localVideoPath);
        CleanupFile(thumbnailPath);
        throw new Exception("Failed to generate thumbnail");

    }

    //// Vidéo : Transcode
    /// <summary>
    ///
    /// </summary>
    /// <param name="localPath"></param>
    /// <returns></returns>
    public async Task<string> TranscodeVideoAsync(Guid videoId, 
        string inputPath, string quality, CancellationToken cancellationToken)
    {
        string localInputPath = await _storageService.DownloadVideoAsync(inputPath, cancellationToken);
        string outputFileName = $"{videoId}_{quality}.mp4";
        string localOutputPath = Path.Combine(_tempPath, outputFileName);

        //(int width, int height, string bitrate) = VideoQualitySetting.Get(quality);
        VideoQualityConfig vquality = VideoQualitySetting.Get(quality);

        string arguments = $"-i \"{localInputPath}\" " +
                       $"-vf scale={vquality.Width}:{vquality.Height} " +
                       $"-c:v libx264 " +
                       $"-b:v {vquality.Bitrate} " +
                       $"-c:a aac " +
                       $"-b:a 128k " +
                       $"-movflags +faststart " +
                       $"\"{localOutputPath}\"";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _settings.ExecutablePath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        //process.Start();
        //await process.WaitForExitAsync();

        process.Start();

        Task<string> outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        Task<string> errorTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        string output = await outputTask;
        string error = await errorTask;

        if (!string.IsNullOrWhiteSpace(error))
        {
            _logger.LogWarning("FFmpeg Transcode error: {Error}", error);
        }

        if (File.Exists(localOutputPath))
        {
            using FileStream stream = File.OpenRead(localOutputPath);
            string toStoragePath = $"{quality}/{outputFileName}";
            string storagePath = await _storageService.UploadVideoAsync(videoId, stream, toStoragePath, "video/mp4", cancellationToken);

            CleanupFile(localInputPath);
            CleanupFile(localOutputPath);

            return storagePath;
        }

        CleanupFile(localInputPath);
        throw new Exception($"Failed to transcode video to {quality}");
    }

    // Audio : Métadonnées
    public async Task<Dictionary<string, string>> GetAudioMetadataAsync(string localPath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffprobe",
                Arguments = $"-v quiet -print_format json -show_format \"{localPath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        // Parser JSON et extraire artist, album, etc.
        return new Dictionary<string, string>();
    }

    private void CleanupFile(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // Log error but don't throw
        }
    }
}
