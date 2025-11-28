using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.MediaBase.UploadMedia;

public sealed record UploadMediaCommand : ICommand<Result<UploadMediaResult>>
{
    public string Type { get; init; } = string.Empty; // "audio" ou "video"
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public Stream MediaFile { get; init; } = null!;
    public string FileName { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public List<string>? Tags { get; init; }
    public Stream? ThumbnailStream { get; init; } // Nouveau
    public string? ThumbnailFileName { get; init; } // Nouveau
    public string? Speaker { get; init; } // spécifique audio
    public string? Category { get; init; }
    public string? Topic { get; init; }
}
