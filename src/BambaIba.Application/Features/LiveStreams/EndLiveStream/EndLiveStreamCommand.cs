// BambaIba.Application/Features/LiveStreams/EndLiveStream/EndLiveStreamCommand.cs
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.LiveStreams.EndLiveStream;

public sealed record EndLiveStreamCommand : ICommand<Result<EndLiveStreamResult>>
{
    public Guid StreamId { get; init; }
    public string StreamerId { get; init; } = string.Empty;
}

public sealed record EndLiveStreamResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }

    public static EndLiveStreamResult Success() => new() { IsSuccess = true };
    public static EndLiveStreamResult Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}
