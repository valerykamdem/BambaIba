namespace BambaIba.SharedKernel.Videos;
public sealed record GetVideosResult(
    List<VideoDto> Videos,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

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
