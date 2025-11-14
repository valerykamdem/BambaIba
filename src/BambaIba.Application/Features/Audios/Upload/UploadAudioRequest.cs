using Microsoft.AspNetCore.Http;

namespace BambaIba.Application.Features.Audios.Upload;
public sealed record UploadAudioRequest
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Artist { get; init; } = string.Empty;
    public string Album { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public IFormFile AudioFile { get; init; } = null!;
    public IFormFile? CoverImage { get; init; } // Optionnel
    public string? Tags { get; init; } // Séparés par virgule
}
