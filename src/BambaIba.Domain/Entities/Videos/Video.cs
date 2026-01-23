using BambaIba.Domain.Entities.MediaBase;
using BambaIba.Domain.Entities.VideoQualities;

namespace BambaIba.Domain.Entities.Videos;

public sealed class Video : Media
{
    public ICollection<VideoQuality> Qualities { get; set; } = [];
}
