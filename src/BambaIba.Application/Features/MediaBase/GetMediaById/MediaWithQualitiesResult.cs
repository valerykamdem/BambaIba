using BambaIba.SharedKernel.Videos;

namespace BambaIba.Application.Features.MediaBase.GetMediaById;
public sealed record MediaWithQualitiesResult
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty; // "audio" ou "video"
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string MediaUrl { get; init; } = string.Empty;
    public string? ThumbnailUrl { get; init; }
    public TimeSpan Duration { get; init; }
    public int PlayCount { get; init; }
    public int LikeCount { get; init; }
    public int DislikeCount { get; init; }
    public int CommentCount { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? PublishedAt { get; init; }
    public Guid UserId { get; init; }
    public bool IsPublic { get; init; }
    public List<string> Tags { get; init; } = [];

    // Spécifique Audio
    public string? Speaker { get; init; }
    public string? Category { get; init; }
    public string? Topic { get; init; }

    // Spécifique Vidéo (optionnel)
    public List<VideoQualityDto> Qualities { get; init; } = [];
}
