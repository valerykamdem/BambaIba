using System;
using System.Collections.Generic;
using System.Text;

namespace BambaIba.Application.Abstractions.Services;

public interface IMediaStatisticsService
{
    Task IncrementCommentCountAsync(Guid mediaId, CancellationToken ct);
    Task DecrementCommentCountAsync(Guid mediaId, CancellationToken ct);
    Task IncrementPlayCountAsync(Guid mediaId, CancellationToken ct);
    Task IncrementLikeCountAsync(Guid mediaId, CancellationToken ct);
    Task DecrementLikeCountAsync(Guid mediaId, CancellationToken ct);
}

