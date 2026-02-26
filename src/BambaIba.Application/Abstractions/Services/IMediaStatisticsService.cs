namespace BambaIba.Application.Abstractions.Services;

public interface IMediaStatisticsService
{
    // Comment
    Task IncrementCommentCountAsync(Guid mediaId, CancellationToken ct);
    Task DecrementCommentCountAsync(Guid mediaId, CancellationToken ct);
    // Play
    Task IncrementPlayCountAsync(Guid mediaId, CancellationToken ct);
    // Like
    Task IncrementLikeCountAsync(Guid mediaId, CancellationToken ct);
    Task DecrementLikeCountAsync(Guid mediaId, CancellationToken ct);
    Task IncrementDislikeCountAsync(Guid mediaId, CancellationToken ct);
    Task DecrementDislikeCountAsync(Guid mediaId, CancellationToken ct);
}

