using System.Data;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Abstractions.Services;
using BambaIba.Domain.Entities.MediaReactions;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace BambaIba.Application.Features.MediaBase.AddReactionToMedia;

public sealed record AddReactionToMediaCommand(
    Guid MediaId,
    ReactionType ReactionType
);

public sealed class AddReactionToMediaHandler(
    IBIDbContext dbContext,
    IUserContextService userContextService,
    IMediaStatisticsService statsService)
{

    public async Task<Result> Handle(AddReactionToMediaCommand command, CancellationToken cancellationToken)
    {
        UserContext userContext = await userContextService.GetCurrentContext();

        // 1. Chercher la réaction existante
        MediaReaction? existingReaction = await dbContext.MediaReactions
            .FirstOrDefaultAsync(r => r.MediaId == command.MediaId && r.UserId == userContext.LocalUserId, cancellationToken);

        if (existingReaction == null)
        {
            // CAS A : Nouvelle réaction
            dbContext.MediaReactions.Add(new MediaReaction
            {
                MediaId = command.MediaId,
                UserId = userContext.LocalUserId,
                ReactionType = command.ReactionType,
                CreatedBy = userContext.LocalUserId.ToString(),
                CreatedAt = DateTime.UtcNow
            });

            if (command.ReactionType == ReactionType.Like)
                await statsService.IncrementLikeCountAsync(command.MediaId, cancellationToken);
            else
                await statsService.IncrementDislikeCountAsync(command.MediaId, cancellationToken);
        }
        else if (existingReaction.ReactionType == command.ReactionType)
        {
            // CAS B : Annuler la réaction
            dbContext.MediaReactions.Remove(existingReaction);

            if (command.ReactionType == ReactionType.Like)
                await statsService.DecrementLikeCountAsync(command.MediaId, cancellationToken);
            else
                await statsService.DecrementDislikeCountAsync(command.MediaId, cancellationToken);
        }
        else
        {
            // CAS C : Changer d'avis
            dbContext.MediaReactions.Remove(existingReaction);

            await dbContext.MediaReactions.AddAsync(new MediaReaction
            {
                MediaId = command.MediaId,
                UserId = userContext.LocalUserId,
                ReactionType = command.ReactionType
            }, cancellationToken);

            if (existingReaction.ReactionType == ReactionType.Like)
                await statsService.DecrementLikeCountAsync(command.MediaId, cancellationToken);
            else
                await statsService.DecrementDislikeCountAsync(command.MediaId, cancellationToken);

            if (command.ReactionType == ReactionType.Like)
                await statsService.IncrementLikeCountAsync(command.MediaId, cancellationToken);
            else
                await statsService.IncrementDislikeCountAsync(command.MediaId, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
