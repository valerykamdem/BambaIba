using BambaIba.Application.Extensions;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.Audios.GetAudios;
public sealed record GetAudiosQuery : IQuery<Result<PagedResult<AudioDto>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Genre { get; init; }
    public string? Search { get; init; }
}

public sealed record GetAudiosResult
{
    public List<AudioDto> Audios { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

public sealed record AudioDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Artist { get; init; } = string.Empty;
    public string Album { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public string CoverImageUrl { get; init; } = string.Empty;
    public TimeSpan Duration { get; init; }
    public int PlayCount { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
