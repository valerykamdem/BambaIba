using BambaIba.Domain.MediaBase;
using BambaIba.Domain.VideoQualities;


namespace BambaIba.Domain.Videos;

public sealed class Video : Media
{
    public ICollection<VideoQuality> Qualities { get; set; } = [];
}
