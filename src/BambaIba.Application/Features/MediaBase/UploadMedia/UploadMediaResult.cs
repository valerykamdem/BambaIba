using BambaIba.Domain.Enums;

namespace BambaIba.Application.Features.MediaBase.UploadMedia;

public sealed record UploadMediaResult
{
    public bool IsSuccess { get; init; }
    public Guid MediaId { get; init; }
    public MediaStatus Status { get; init; }
    public string? Messaqge { get; init; }
    public string? ErrorMessage { get; init; }

    public static UploadMediaResult Success(Guid mediaId, 
        MediaStatus status, string message)
        => new()
        {
            IsSuccess = true,
            MediaId = mediaId,
            Status = status,
            Messaqge = message
        };

    public static UploadMediaResult Failure(string errorMessage)
        => new()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
}
