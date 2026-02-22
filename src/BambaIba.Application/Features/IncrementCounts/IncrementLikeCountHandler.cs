using System;
using System.Collections.Generic;
using System.Text;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.MediaStats;

namespace BambaIba.Application.Features.IncrementCounts;

public sealed record IncrementLikeCountCommand(Guid MediaId);

public sealed class IncrementLikeCountHandler
{
    public static async Task Handle(
        IncrementLikeCountCommand command, 
        IBIDbContext dbContext,
        CancellationToken cancellationToken)
    {
        MediaStat stat = await dbContext.MediaStats
            .FindAsync(new object?[] { command.MediaId }, cancellationToken: cancellationToken);

        if (stat == null)
            return;
        stat.LikeCount++;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
