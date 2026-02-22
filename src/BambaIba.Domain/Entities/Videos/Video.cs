using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.VideoQualities;

namespace BambaIba.Domain.Entities.Videos;

public sealed class Video : MediaAsset
{
    public ICollection<VideoQuality> Qualities { get; set; } = [];
}
