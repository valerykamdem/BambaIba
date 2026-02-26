using BambaIba.Application.Abstractions.Services;
using BambaIba.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BambaIba.Infrastructure.Services;

public class MediaStatisticsService : IMediaStatisticsService
{
    private readonly BIDbContext _db;
    private readonly ILogger<MediaStatisticsService> _logger;

    public MediaStatisticsService(BIDbContext db, ILogger<MediaStatisticsService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task IncrementCommentCountAsync(Guid mediaId, CancellationToken ct)
    {
        // Mise à jour atomique dans Postgres pour le feed principal
        int rows = await _db.MediaAssets
            .Where(m => m.Id == mediaId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Stat.CommentCount, m => m.Stat.CommentCount + 1),
            ct);

        if (rows == 0)
            _logger.LogWarning("Media {MediaId} not found for comment count increment", mediaId);
    }

    public async Task DecrementCommentCountAsync(Guid mediaId, CancellationToken ct)
    {
        await _db.MediaAssets
        .Where(m => m.Id == mediaId && m.Stat.CommentCount > 0)
        .ExecuteUpdateAsync(setters => setters
            .SetProperty(m => m.Stat.CommentCount, m => m.Stat.CommentCount - 1)
            .SetProperty(m => m.UpdatedAt, DateTime.UtcNow),
        ct);
    }

    public async Task IncrementPlayCountAsync(Guid mediaId, CancellationToken ct)
    {
        await _db.MediaAssets
            .Where(m => m.Id == mediaId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Stat.PlayCount, m => m.Stat.PlayCount + 1),
            ct);
    }

    public async Task IncrementLikeCountAsync(Guid mediaId, CancellationToken ct)
    {
        // Mise à jour atomique dans Postgres pour le feed principal
        int rows = await _db.MediaAssets
            .Where(m => m.Id == mediaId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Stat.LikeCount, m => m.Stat.LikeCount + 1),
            ct);

        if (rows == 0)
            _logger.LogWarning("Media {MediaId} not found for like count increment", mediaId);
    }

    public async Task DecrementLikeCountAsync(Guid mediaId, CancellationToken ct)
    {
        await _db.MediaAssets
           .Where(m => m.Id == mediaId)
           .ExecuteUpdateAsync(setters => setters
               .SetProperty(m => m.Stat.LikeCount, m => m.Stat.LikeCount - 1),
           ct);
    }

    public async Task IncrementDislikeCountAsync(Guid mediaId, CancellationToken ct)
    {
        await _db.MediaAssets
            .Where(m => m.Id == mediaId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Stat.DislikeCount, m => m.Stat.DislikeCount + 1)
                .SetProperty(m => m.UpdatedAt, DateTime.UtcNow),
            ct);
    }

    public async Task DecrementDislikeCountAsync(Guid mediaId, CancellationToken ct)
    {
        await _db.MediaAssets
            .Where(m => m.Id == mediaId && m.Stat.DislikeCount > 0)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Stat.DislikeCount, m => m.Stat.DislikeCount - 1)
                .SetProperty(m => m.UpdatedAt, DateTime.UtcNow),
            ct);
    }
}
