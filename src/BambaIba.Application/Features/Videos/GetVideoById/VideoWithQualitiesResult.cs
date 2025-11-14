using BambaIba.Domain.VideoQualities;
using BambaIba.SharedKernel.Videos;

namespace BambaIba.Application.Features.Videos.GetVideoById;
public sealed record VideoWithQualitiesResult
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string VideoUrl { get; init; } = string.Empty;      // ← URL présignée
    public string ThumbnailUrl { get; init; } = string.Empty;  // ← URL présignée
    public TimeSpan Duration { get; init; }
    public int ViewCount { get; init; }
    public int LikeCount { get; init; }
    public int DislikeCount { get; init; }
    public List<VideoQualityDto> Qualities { get; init; } = [];
    //public List<VideoQuality> Qualities { get; init; } = [];
    public DateTime? CreatedAt { get; init; }
    public Guid UserId { get; init; }
    public int CommentCount { get; init; }  // ← Juste le nombre
    public List<string> Tags { get; init; } = [];
    // ...
}
