namespace BambaIba.Application.Common.Dtos;
public record VideoDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ThumbnailUrl { get; init; } = string.Empty;
    public TimeSpan Duration { get; init; }
    public int ViewCount { get; init; }
    public int LikeCount { get; init; }
    public string UserId { get; init; } = string.Empty;
}
