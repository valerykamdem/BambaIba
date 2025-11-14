using BambaIba.Domain.Enums;

namespace BambaIba.Application.Features.Videos.Upload;

public sealed record UploadVideoResult
{
    public bool IsSuccess { get; init; }
    public Guid VideoId { get; init; }
    public MediaStatus Status { get; init; }
    public string? Messaqge { get; init; }
    public string? ErrorMessage { get; init; }

    public static UploadVideoResult Success(Guid videoId, 
        MediaStatus status, string message)
        => new()
        {
            IsSuccess = true,
            VideoId = videoId,
            Status = status,
            Messaqge = message
        };

    public static UploadVideoResult Failure(string errorMessage)
        => new()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
}
