
using BambaIba.Domain.Enums;

namespace BambaIba.Application.Features.LiveStreams.StartLiveStream;
public sealed record StartLiveStreamRequest
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public sealed record GetLiveStreamsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed record LiveStreamDetailDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string StreamerId { get; init; } = string.Empty;
    public string HlsUrl { get; init; } = string.Empty;
    public LiveStreamStatus Status { get; init; }
    public int ViewerCount { get; init; }
    public DateTime? StartedAt { get; init; }
}
