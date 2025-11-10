using Microsoft.AspNetCore.Http;

namespace BambaIba.Application.Features.Videos.Upload;

public sealed record UploadVideoRequest
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IFormFile File { get; init; } = null!;
    public List<string>? Tags { get; init; }
}

//// Pour query parameters
//public record GetVideosRequest
//{
//    public int Page { get; init; } = 1;
//    public int PageSize { get; init; } = 20;
//    public string? Search { get; init; }
//    public string? Category { get; init; }
//    public string? SortBy { get; init; }
//}
