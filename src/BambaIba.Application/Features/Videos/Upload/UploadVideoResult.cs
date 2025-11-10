using BambaIba.SharedKernel.Enums;

namespace BambaIba.Application.Features.Videos.Upload;

public sealed record UploadVideoResult
{
    public bool IsSuccess { get; init; }
    public Guid VideoId { get; init; }
    public VideoStatus Status { get; init; }
    public string? ErrorMessage { get; init; }

    public static UploadVideoResult Success(Guid videoId, VideoStatus status)
        => new()
        {
            IsSuccess = true,
            VideoId = videoId,
            Status = status
        };

    public static UploadVideoResult Failure(string errorMessage)
        => new()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
}
