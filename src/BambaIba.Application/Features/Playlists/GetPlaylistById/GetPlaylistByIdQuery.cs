using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.Playlists.GetPlaylistById;
public sealed record GetPlaylistByIdQuery : IQuery<Result<PlaylistDetailResult?>>
{
    public Guid PlaylistId { get; init; }
    //public string? CurrentUserId { get; init; }
}

public record PlaylistDetailResult
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsPublic { get; init; }
    public int VideoCount { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<PlaylistVideoDto> Videos { get; init; } = new();
}

public record PlaylistVideoDto
{
    public Guid VideoId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ThumbnailUrl { get; init; } = string.Empty;
    public TimeSpan Duration { get; init; }
    public int Position { get; init; }
    public DateTime AddedAt { get; init; }
}
