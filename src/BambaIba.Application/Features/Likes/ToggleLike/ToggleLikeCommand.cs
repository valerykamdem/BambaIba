using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.Likes.ToggleLike;
public sealed record ToggleLikeCommand(
    Guid MediaId,
    bool IsLiked) : ICommand<Result<ToggleLikeResult>>;


public sealed record ToggleLikeResult
{
    public bool IsSuccess { get; init; }
    public int LikeCount { get; init; }
    public int DislikeCount { get; init; }
    public UserLikeStatus UserStatus { get; init; }
    public string? ErrorMessage { get; init; }

    public static ToggleLikeResult Success(int likeCount, int dislikeCount, UserLikeStatus status)
        => new()
        {
            IsSuccess = true,
            LikeCount = likeCount,
            DislikeCount = dislikeCount,
            UserStatus = status
        };

    public static ToggleLikeResult Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}

public sealed record UserLikeStatus
{
    public bool HasLiked { get; init; }
    public bool HasDisliked { get; init; }
}
