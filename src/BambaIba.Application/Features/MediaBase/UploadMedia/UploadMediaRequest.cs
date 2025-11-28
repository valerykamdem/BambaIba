using Microsoft.AspNetCore.Http;

namespace BambaIba.Application.Features.MediaBase.UploadMedia;

public sealed record UploadMediaRequest
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Speaker { get; init; } // spécifique audio
    public string? Category { get; init; }
    public string? Topic { get; init; }
    public IFormFile MediaFile { get; init; } = null!; 
    public IFormFile? ThumbnailFile { get; init; } // Nouveau
    public List<string>? Tags { get; init; }
}
