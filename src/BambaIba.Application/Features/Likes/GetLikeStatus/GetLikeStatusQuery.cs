using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.Likes.GetLikeStatus;
public sealed record GetLikeStatusQuery(Guid VideoId) : IQuery<Result<GetLikeStatusResult>>;

public record GetLikeStatusResult
{
    public bool HasLiked { get; init; }
    public bool HasDisliked { get; init; }
    public int LikeCount { get; init; }
    public int DislikeCount { get; init; }
}
