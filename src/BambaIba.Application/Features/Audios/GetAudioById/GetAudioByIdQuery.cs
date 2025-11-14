using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Videos;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.Audios.GetAudioById;
public sealed record GetAudioByIdQuery(Guid AudioId) : IQuery<Result<AudioDetailResult>>;

public sealed record AudioDetailResult
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string AudioUrl { get; init; } = string.Empty;      // ← URL présignée
    public string CoverUrl { get; init; } = string.Empty;  // ← URL présignée
    public TimeSpan Duration { get; init; }
    public int PlayCount { get; init; }
    public int LikeCount { get; init; }
    public int DislikeCount { get; init; }
    public DateTime? CreatedAt { get; init; }
    public Guid UserId { get; init; }
    public int CommentCount { get; init; }  // ← Juste le nombre
    // ...
}
