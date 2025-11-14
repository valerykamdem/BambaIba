// BambaIba.Application/Features/LiveStreams/GetLiveStreams/GetLiveStreamsQuery.cs
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.LiveStreams.GetLiveStreams;

public sealed record GetLiveStreamsQuery : IQuery<Result<GetLiveStreamsResult>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public bool OnlyLive { get; init; } = true;
}

public sealed record GetLiveStreamsResult
{
    public List<LiveStreamDto> Streams { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

public sealed record LiveStreamDto
{
    public Guid Id { get; init; }
    public string StreamerId { get; init; } = string.Empty;
    public string StreamerName { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string ThumbnailUrl { get; init; } = string.Empty;
    public LiveStreamStatus Status { get; init; }
    public int ViewerCount { get; init; }
    public DateTime? StartedAt { get; init; }
}
