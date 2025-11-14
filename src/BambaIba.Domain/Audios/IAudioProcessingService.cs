
namespace BambaIba.Domain.Audios;
public interface IAudioProcessingService
{
    Task<TimeSpan> GetAudioDurationAsync(string localAudioPath);
}
