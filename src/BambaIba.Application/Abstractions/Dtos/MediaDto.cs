
namespace BambaIba.Application.Abstractions.Dtos;

public sealed record MediaDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Type { get; init; } = string.Empty; // "audio" ou "video"
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    //public string Url { get; init; } = string.Empty;
    public string ThumbnailUrl { get; init; } = string.Empty;
    public TimeSpan Duration { get; init; }
    public int LikeCount { get; init; }
    public int DislikeCount { get; init; }
    public int PlayCount { get; init; }
    public int CommentCount { get; init; }
    public bool IsPublic { get; init; }
    public DateTime? PublishedAt { get; init; }
    public List<string> Tags { get; init; } = [];
    public string Category { get; init; } = string.Empty;
    public string Topic { get; init; } = string.Empty;
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string Speaker { get; init; } = string.Empty;
    public string Language { get; init; } = string.Empty;
}
