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
    //IHttpContextAccessor httpContextAccessor,
    IMediaStatisticsService statsService
    /*ILogger<AddReactionToMediaHandler> logger*/)
{

    public async Task<Result> Handle(AddReactionToMediaCommand command, CancellationToken cancellationToken)
    {
        UserContext userContext = await userContextService
                .GetCurrentContext();

        IDbTransaction transaction = await dbContext.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Chercher la réaction existante
            MediaReaction? existingReaction = await dbContext.MediaReactions
                .FirstOrDefaultAsync(r => r.MediaId == command.MediaId && r.UserId == userContext.LocalUserId, cancellationToken);

            if (existingReaction == null)
            {
                // CAS A : Nouvelle réaction (Ajout)
                dbContext.MediaReactions.Add(new MediaReaction
                {
                    MediaId = command.MediaId,
                    UserId = userContext.LocalUserId,
                    ReactionType = command.ReactionType
                });

                // Mise à jour compteur
                if (command.ReactionType == ReactionType.Like)
                    await statsService.IncrementLikeCountAsync(command.MediaId, cancellationToken);
                else
                    await statsService.IncrementDislikeCountAsync(command.MediaId, cancellationToken);
            }
            else if (existingReaction.ReactionType == command.ReactionType)
            {
                // CAS B : Annuler la réaction (Toggle off)
                dbContext.MediaReactions.Remove(existingReaction);

                // Mise à jour compteur
                if (command.ReactionType == ReactionType.Like)
                    await statsService.DecrementLikeCountAsync(command.MediaId, cancellationToken);
                else
                    await statsService.DecrementDislikeCountAsync(command.MediaId, cancellationToken);
            }
            else
            {
                // CAS C : Changer d'avis (Toggle switch)
                // On supprime l'ancienne et on ajoute la nouvelle pour garder l'historique propre
                dbContext.MediaReactions.Remove(existingReaction);

                await dbContext.MediaReactions.AddAsync(new MediaReaction
                {
                    MediaId = command.MediaId,
                    UserId = userContext.LocalUserId,
                    ReactionType = command.ReactionType
                }, cancellationToken);

                // Ajuster les deux compteurs
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
            transaction.Commit();

            return Result.Success();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}
