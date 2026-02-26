using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.MediaChannels;
using BambaIba.Domain.Entities.Roles;
using BambaIba.SharedKernel;

namespace BambaIba.Application.Features.MediaChannels.CreateMediaChannel;


public sealed record CreateMediaChannelCommand(
    string Name,
    string Handle,
    string? Description
);

public sealed class CreateMediaChannelHandler(
    IBIDbContext dbContext,
    IUserContextService userContextService)
{

    public async Task<Result<Guid>> Handle(CreateMediaChannelCommand cmd, CancellationToken ct)
    {
        // 1. Récupérer l'utilisateur connecté
        UserContext userContext = await userContextService
            .GetCurrentContext();

        // --- SECURITY CHECK : Role Verification ---
        // Only "Creator" (or "Admin") can create channels.
        if (userContext.Role != RoleNames.Creator && userContext.Role != RoleNames.Admin)
        {
            return Result.Failure<Guid>(
                Error.Unauthorized("Not.Allow","Access Denied: Only Users with 'Creator' role can create channels.")
            );
        }

        //// --- CAPACITY CHECK (Plan Limits) ---
        //// Even a Creator has limits based on their paid plan.
        //int currentChannelsCount = await dbContext.MediaChannels
        //    .CountAsync(c => c.UserId == userContext.LocalUserId, ct);

        //int maxChannelsAllowed = userContext.SubscriptionPlan switch
        //{
        //    "Free" => 1,
        //    "Pro" => 5,
        //    "Enterprise" => 20,
        //    _ => 1
        //};

        //if (currentChannelsCount >= maxChannelsAllowed)
        //{
        //    return Result.Failure<Guid>(Error.Forbidden($"Plan Limit Reached: You can only have {maxChannelsAllowed} channels on the '{userContext.SubscriptionPlan}' plan."));
        //}

        // --- CREATION ---
        var channel = new MediaChannel
        {
            //Id = Guid.CreateVersion7(),
            UserId = userContext.LocalUserId,
            Name = cmd.Name,
            Handle = cmd.Handle,
            Description = cmd.Description
        };

        dbContext.MediaChannels.Add(channel);
        await dbContext.SaveChangesAsync(ct);

        return Result.Success(channel.Id);
    }
}
