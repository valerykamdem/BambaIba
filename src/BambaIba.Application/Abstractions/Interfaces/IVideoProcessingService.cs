namespace BambaIba.Application.Abstractions.Interfaces;

public interface IVideoProcessingService
{
    Task<TimeSpan> GetVideoDurationAsync(string videoPath);
    Task<string> GenerateThumbnailAsync(Guid videoId, string videoPath);
    Task<string> TranscodeVideoAsync(Guid videoId, string inputPath, string quality);
}
