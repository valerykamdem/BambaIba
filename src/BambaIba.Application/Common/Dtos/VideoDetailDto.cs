namespace BambaIba.Application.Common.Dtos;

public record VideoDetailDto : VideoDto
{
    public string VideoUrl { get; init; } = string.Empty;
    public int DislikeCount { get; init; }
    public int CommentCount { get; init; }
    public List<string> Tags { get; init; } = [];
    public List<VideoQualityDto> Qualities { get; init; } = [];
}
