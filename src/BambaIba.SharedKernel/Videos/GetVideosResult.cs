namespace BambaIba.SharedKernel.Videos;
public sealed record GetVideosResult
{
    public List<VideoDto> Videos { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

public record VideoDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ThumbnailUrl { get; init; } = string.Empty;
    public TimeSpan Duration { get; init; }
    public int ViewCount { get; init; }
    public int LikeCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid UserId { get; init; }
}
