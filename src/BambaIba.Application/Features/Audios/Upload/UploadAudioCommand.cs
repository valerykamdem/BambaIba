using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.Audios.Upload;
public sealed record UploadAudioCommand : ICommand<Result<UploadAudioResult>>
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Artist { get; init; } = string.Empty;
    public string Album { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public Stream AudioStream { get; init; } = null!;
    public string AudioFileName { get; init; } = string.Empty;
    public long AudioFileSize { get; init; }
    public string AudioContentType { get; init; } = string.Empty;
    public Stream? CoverImageStream { get; init; } // Optionnel
    public string? CoverImageFileName { get; init; }
    public List<string>? Tags { get; init; }
}

public record UploadAudioResult
{
    public bool IsSuccess { get; init; }
    public Guid AudioId { get; init; }
    public MediaStatus Status { get; init; }
    public string? ErrorMessage { get; init; }

    public static UploadAudioResult Success(Guid audioId, MediaStatus status)
        => new() { IsSuccess = true, AudioId = audioId, Status = status };

    public static UploadAudioResult Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}
