// BambaIba.Infrastructure/Services/FFmpegMediaProcessingService.cs
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Settings;
using BambaIba.Domain.Enums;
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
            string uploadImg = await _storageService.UploadImageAsync(videoId, stream, storagePath, MediaType.VideoThumbnail);

            CleanupFile(localVideoPath);
            CleanupFile(thumbnailPath);

            return uploadImg;
        }

        CleanupFile(localVideoPath);
        CleanupFile(thumbnailPath);
        throw new Exception("Failed to generate thumbnail");

    }

    //// Vidéo : Transcode
    /// <summary>
    /// 
    /// </summary>
    /// <param name="videoId"></param>
    /// <param name="localInputPath"></param>
    /// <param name="quality"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
    public async Task<string> TranscodeVideoAsync(
    Guid videoId,
    string localInputPath,
    string quality,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(localInputPath) || !File.Exists(localInputPath))
            throw new FileNotFoundException("Input video not found", localInputPath);

        VideoQualityConfig vquality = VideoQualitySetting.Get(quality);

        // Configuration pour streaming direct (Pipe vers S3)
        // On ne PEUT PAS utiliser +faststart avec un pipe.
        // On utilise fragmented MP4 (fMP4) qui est compatible streaming.
        string arguments =
            $"-i \"{localInputPath}\" " +
            $"-vf scale={vquality.Width}:{vquality.Height} " +
            $"-c:v libx264 -preset veryfast " +
            $"-b:v {vquality.Bitrate} " +
            $"-c:a aac -b:a 128k " +
            $"-movflags frag_keyframe+empty_moov+default_base_moof " + // Important pour le streaming
            $"-f mp4 -"; // Le tiret "-" signifie "écrire vers la sortie standard (stdout)"

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _settings.ExecutablePath,
                Arguments = arguments,
                RedirectStandardOutput = true, // On capture la sortie
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        _logger.LogInformation("Starting FFmpeg Pipe for {VideoId} {Quality}", videoId, quality);

        process.Start();

        // Capture stderr pour le logs (sans bloquer)
        Task<string> stderrTask = process.StandardError.ReadToEndAsync();

        // Le flux vidéo sort directement de FFmpeg
        await using Stream ffmpegOutput = process.StandardOutput.BaseStream;

        string outputFileName = $"{videoId}_{quality}.mp4";
        string storageKey = $"{videoId}/{quality}/{outputFileName}";

        // Upload direct vers SeaweedFS pendant que FFmpeg tourne
        string storagePath = await _storageService.UploadVideoStreamAsync(
            videoId,
            ffmpegOutput,
            storageKey,
            "video/mp4",
            cancellationToken);

        // Attendre la fin de FFmpeg
        await process.WaitForExitAsync(cancellationToken);

        string stderr = await stderrTask;

        if (process.ExitCode != 0)
        {
            _logger.LogError("FFmpeg failed: {Error}", stderr);
            throw new Exception($"FFmpeg failed ({quality}): {stderr}");
        }

        return storageKey;
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
        return [];
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
