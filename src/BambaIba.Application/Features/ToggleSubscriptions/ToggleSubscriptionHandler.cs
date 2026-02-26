using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.UserSubscriptions;
using BambaIba.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace BambaIba.Application.Features.ToggleSubscriptions;

// Commande
public sealed record ToggleSubscriptionCommand(Guid FollowingUserId);

// Handler
public sealed class ToggleSubscriptionHandler(
    IBIDbContext dbContext,
    IUserContextService userContextService)
{

    public async Task<Result> Handle(ToggleSubscriptionCommand command, CancellationToken cancellationToken)
    {
        UserContext userContext = await userContextService
                 .GetCurrentContext();

        UserSubscription? existing = await dbContext.UserSubscriptions
            .FirstOrDefaultAsync(s => s.FollowerId == userContext.LocalUserId && s.ChannelId == command.FollowingUserId, cancellationToken);

        if (existing == null)
        {
            // S'abonner
            dbContext.UserSubscriptions.Add(new UserSubscription
            {
                FollowerId = userContext.LocalUserId,
                ChannelId = command.FollowingUserId
            });
        }
        else
        {
            // Se désabonner
            dbContext.UserSubscriptions.Remove(existing);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

