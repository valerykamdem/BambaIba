using System.Diagnostics;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Settings;
using BambaIba.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BambaIba.Infrastructure.Repositories;
public class FFmpegVideoProcessingService : IVideoProcessingService
{
    private readonly FFmpegSettings _options;
    private readonly IVideoStorageService _storageService;
    private readonly string _tempPath;
    private readonly ILogger<FFmpegVideoProcessingService> _logger;

    public FFmpegVideoProcessingService(
        IOptions<FFmpegSettings> options,
        IVideoStorageService storageService,
        ILogger<FFmpegVideoProcessingService> logger)
    {
        _options = options.Value;
        _storageService = storageService;
        _logger = logger;
        _tempPath = _options.TempDirectory;
        _tempPath = Path.Combine(Path.GetTempPath(), "bambaiba_videos");

        Directory.CreateDirectory(_tempPath);

        // ✅ Vérifier que FFmpeg existe
        VerifyFFmpegInstallation();
    }

    private void VerifyFFmpegInstallation()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _options.ExecutablePath,
                    Arguments = "-version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"FFmpeg not found at: {_options.ExecutablePath}");
            }

            _logger.LogInformation("FFmpeg verified successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FFmpeg verification failed");
            throw;
        }
    }

    public async Task<TimeSpan> GetVideoDurationAsync(string videoPath)
    {
        string localPath = await DownloadVideoAsync(videoPath);

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _options.ExecutablePath,
                Arguments = $"-i \"{localPath}\" 2>&1 | grep Duration",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string output = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        // Parse duration from FFmpeg output
        System.Text.RegularExpressions.Match durationMatch = System.Text.RegularExpressions.Regex.Match(
            output, @"Duration: (\d{2}):(\d{2}):(\d{2})\.(\d{2})");

        if (durationMatch.Success)
        {
            int hours = int.Parse(durationMatch.Groups[1].Value);
            int minutes = int.Parse(durationMatch.Groups[2].Value);
            int seconds = int.Parse(durationMatch.Groups[3].Value);
            int milliseconds = int.Parse(durationMatch.Groups[4].Value) * 10;

            return new TimeSpan(0, hours, minutes, seconds, milliseconds);
        }

        CleanupFile(localPath);
        return TimeSpan.Zero;
    }

    public async Task<string> GenerateThumbnailAsync(Guid videoId, string videoPath)
    {
        string localVideoPath = await DownloadVideoAsync(videoPath);
        string thumbnailPath = Path.Combine(_tempPath, $"{videoId}_thumb.jpg");

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _options.ExecutablePath,
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
            string storagePath = $"thumbnails/{videoId}.jpg";
            await _storageService.UploadFileAsync(storagePath, stream, "image/jpeg");

            CleanupFile(localVideoPath);
            CleanupFile(thumbnailPath);

            return storagePath;
        }

        CleanupFile(localVideoPath);
        throw new Exception("Failed to generate thumbnail");
    }


    public async Task<string> TranscodeVideoAsync(Guid videoId, string inputPath, string quality)
    {
        string localInputPath = await DownloadVideoAsync(inputPath);
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
                FileName = _options.ExecutablePath,
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

        Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        string output = await outputTask;
        string error = await errorTask;

        if (!string.IsNullOrWhiteSpace(error))
        {
            _logger.LogWarning("FFmpeg Transcode error: {Error}", error);
        }

        if (File.Exists(localOutputPath))
        {
            using FileStream stream = File.OpenRead(localOutputPath);
            string storagePath = $"videos/{videoId}/{quality}/{outputFileName}";
            await _storageService.UploadFileAsync(storagePath, stream, "video/mp4");

            CleanupFile(localInputPath);
            CleanupFile(localOutputPath);

            return storagePath;
        }

        CleanupFile(localInputPath);
        throw new Exception($"Failed to transcode video to {quality}");
    }

    // L'implémentation de la fonction
    private async Task<string> DownloadVideoAsync(string videoPath)
    {
        // 1. Détermine le chemin local de destination
        // videoPath est l'objectName dans MinIO, ex: "videos/guid/fichier.mp4"
        // Le nom de fichier est la dernière partie du path.
        string fileName = Path.GetFileName(videoPath);
        string localPath = Path.Combine(_tempPath, fileName);

        // 2. Si le fichier existe déjà localement
        if (File.Exists(localPath))
            return localPath;

        // 3. Télécharger depuis MinIO
        try
        {
            // 🚨 IMPORTANT : videoPath est le nom de l'objet MinIO
            await _storageService.DownloadObjectAsync(videoPath, localPath);
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            // Gérer le cas où l'objet n'existe pas dans MinIO
            throw new FileNotFoundException($"Le fichier vidéo '{videoPath}' n'a pas été trouvé dans MinIO.");
        }
        catch (Exception ex)
        {
            // Gérer d'autres erreurs de téléchargement
            Console.WriteLine($"Erreur lors du téléchargement : {ex.Message}");
            throw;
        }

        // 4. Retourner le chemin local
        return localPath;
    }

    //private (int width, int height, string bitrate) GetQualitySettings(string quality)
    //{
    //    return quality switch
    //    {
    //        "240p" => (520, 240, "300k"),
    //        "360p" => (640, 360, "800k"),
    //        "480p" => (854, 480, "1200k"),
    //        _ => (640, 360, "800k")
    //        //"720p" => (1280, 720, "2500k"),
    //        //"1080p" => (1920, 1080, "5000k"),
    //        //_ => (1280, 720, "2500k")
    //    };
    //}

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
