// BambaIba.Application/Features/LiveStreams/StartLiveStream/StartLiveStreamCommand.cs

using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.LiveStreams.StartLiveStream;

public sealed record StartLiveStreamCommand : ICommand<Result<StartLiveStreamResult>>
{
    public string StreamerId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public sealed record StartLiveStreamResult
{
    public bool IsSuccess { get; init; }
    public Guid StreamId { get; init; }
    public string StreamKey { get; init; } = string.Empty;
    public string RtmpUrl { get; init; } = string.Empty;
    public string? ErrorMessage { get; init; }

    public static StartLiveStreamResult Success(Guid streamId, string streamKey, string rtmpUrl)
        => new()
        {
            IsSuccess = true,
            StreamId = streamId,
            StreamKey = streamKey,
            RtmpUrl = rtmpUrl
        };

    public static StartLiveStreamResult Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}
