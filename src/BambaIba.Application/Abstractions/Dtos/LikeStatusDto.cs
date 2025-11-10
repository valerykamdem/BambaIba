namespace BambaIba.Application.Abstractions.Dtos;
public record LikeStatusDto
{
    public Guid VideoId { get; init; }
    public int LikeCount { get; init; }
    public int DislikeCount { get; init; }
    public string? UserLikeStatus { get; init; }
}
