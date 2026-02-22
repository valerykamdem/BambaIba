using System;
using System.Collections.Generic;
using System.Text;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.MediaStats;

namespace BambaIba.Application.Features.IncrementCounts;

public sealed record IncrementPlayCountCommand(Guid MediaId);

public sealed class IncrementPlayCountHandler
{
    public static async Task Handle(
        IncrementPlayCountCommand command, 
        IBIDbContext dbContext,
        CancellationToken cancellationToken)
    {
        MediaStat stat = await dbContext.MediaStats
            .FindAsync(new object?[] { command.MediaId }, cancellationToken: cancellationToken);

        if (stat == null)
            return;
        stat.PlayCount++;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
