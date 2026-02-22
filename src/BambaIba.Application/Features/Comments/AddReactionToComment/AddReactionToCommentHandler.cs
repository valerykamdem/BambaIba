using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Mongo.Comments;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace BambaIba.Application.Features.Comments.AddReactionToComment;

public sealed record AddReactionToCommentCommand(
    string CommentId,
    ReactionType ReactionType
);

public sealed class AddReactionToCommentHandler(
    IUserContextService userContextService,
    IHttpContextAccessor httpContextAccessor,
    IBIMongoContext mongoContext,
    IMongoClient mongoClient,
    ILogger<AddReactionToCommentHandler> logger)
{

    public async Task<Result> Handle(AddReactionToCommentCommand command, CancellationToken cancellationToken)
    {
        UserContext userContext = await userContextService
                .GetCurrentContext(httpContextAccessor.HttpContext);

        IClientSessionHandle session = await mongoClient.StartSessionAsync(cancellationToken: cancellationToken);

        try
        {
            session.StartTransaction(); // Début de la transaction ACID

            // 1. Chercher la réaction existante
            FilterDefinition<CommentReaction> filter = Builders<CommentReaction>.Filter.And(
                Builders<CommentReaction>.Filter.Eq(r => r.CommentId, command.CommentId),
                Builders<CommentReaction>.Filter.Eq(r => r.UserId, userContext.LocalUserId.ToString())
            );

            CommentReaction existingReaction = await mongoContext.CommentReactions.Find(filter).FirstOrDefaultAsync(cancellationToken);

            FilterDefinition<Comment> commentFilter = Builders<Comment>.Filter.Eq(c => c.Id, command.CommentId);
            // On déclare update comme un UpdateDefinition, pas un Builder
            UpdateDefinition<Comment>? update = null;

            if (existingReaction == null)
            {
                // CAS A : Nouvelle réaction
                update = command.ReactionType == ReactionType.Like
                    ? Builders<Comment>.Update.Inc(c => c.LikeCount, 1)
                    : Builders<Comment>.Update.Inc(c => c.DislikeCount, 1);
            }
            else if (existingReaction.Type == command.ReactionType)
            {
                // CAS B : Annuler la réaction
                update = command.ReactionType == ReactionType.Like
                    ? Builders<Comment>.Update.Inc(c => c.LikeCount, -1)
                    : Builders<Comment>.Update.Inc(c => c.DislikeCount, -1);
            }
            else
            {
                // CAS C : Changer de camp (Like -> Dislike)
                // Ici on doit incrémenter l'un et décrémenter l'autre
                update = Builders<Comment>.Update
                    .Inc(c => c.LikeCount, command.ReactionType == ReactionType.Like ? 1 : -1)
                    .Inc(c => c.DislikeCount, command.ReactionType == ReactionType.Dislike ? 1 : -1);
            }

            // Appliquer la mise à jour au commentaire
            await mongoContext.Comments.UpdateOneAsync(session, commentFilter, update, cancellationToken: cancellationToken);

            await session.CommitTransactionAsync(cancellationToken); // Valider tout
            return Result.Success();
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync(cancellationToken); // Tout annuler si erreur
            throw;
        }
    }
}
