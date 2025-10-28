using BambaIba.Application.Settings;
using Microsoft.Extensions.Options;

namespace BambaIba.Infrastructure.Services;
public class VideoProcessingService
{
    private readonly IOptionsSnapshot<VideoProcessingSettings> _settings;

    public VideoProcessingService(IOptionsSnapshot<VideoProcessingSettings> settings)
    {
        _settings = settings;  // ← Refresh automatique
    }

    public void ProcessVideo()
    {
        long maxSize = _settings.Value.MaxFileSizeBytes;  // ← Toujours à jour
    }
}
