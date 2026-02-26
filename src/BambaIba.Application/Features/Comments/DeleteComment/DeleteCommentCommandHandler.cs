using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Abstractions.Services;
using BambaIba.Domain.Entities.Mongo.Comments;
using BambaIba.SharedKernel;
using MongoDB.Driver;

namespace BambaIba.Application.Features.Comments.DeleteComment;

public sealed record DeleteCommentCommand(string CommentId);

public sealed class DeleteCommentCommandHandler(
    IBIMongoContext mongoContext,
    IUserContextService userContextService,
    IMediaStatisticsService statsService,
    IMongoClient mongoClient
    /*ILogger<DeleteCommentCommandHandler> logger*/)
{

    public async Task<Result> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
    {
        UserContext userContext = await userContextService
                .GetCurrentContext();

        IClientSessionHandle session = mongoClient.StartSession(cancellationToken: cancellationToken);

        try
        {
            session.StartTransaction();

            // 1. Trouver le commentaire pour vérifier la propriété et les réponses
            FilterDefinition<Comment> commentFilter = Builders<Comment>.Filter.Eq(c => c.Id, command.CommentId);
            Comment comment = await mongoContext.Comments.Find(commentFilter).FirstOrDefaultAsync(cancellationToken);

            if (comment == null)
                return Result.Failure(Error.NotFound("","Comment not find"));

            // SÉCURITÉ : Vérifier que c'est bien son commentaire (ou un admin)
            if (comment.UserId != userContext.LocalUserId.ToString())
            {
                // Pour un admin, tu pourrais ajouter une vérification de rôle ici
                return Result.Failure(Error.Problem("403", "You can only delete your own comments"));
            }

            // INTÉGRITÉ DES DONNÉES : Vérifier s'il y a des réponses
            // On ne permet pas de supprimer un commentaire qui a des enfants (réponses)
            FilterDefinition<Comment> repliesFilter = Builders<Comment>.Filter.Eq(c => c.ParentId, command.CommentId);
            long repliesCount = await mongoContext.Comments.CountDocumentsAsync(repliesFilter, cancellationToken: cancellationToken);

            if (repliesCount > 0)
            {
                // Option A : Erreur bloquante (Mon choix actuel)
                return Result.Failure(Error.Conflict("409", "Cannot delete comment: It has replies. Delete replies first."));

                // Option B (Si tu veux Soft Delete) :
                // À la place de l'erreur, on ferait un Update pour mettre Content = "[Deleted]"
            }

            //// 2. Supprimer le commentaire
            //await mongoContext.Comments.DeleteOneAsync(commentFilter, cancellationToken);

            //// 3. Supprimer toutes les réactions associées (Likes/Dislikes)
            //await mongoContext.CommentReactions.DeleteManyAsync(r => r.CommentId == command.CommentId, cancellationToken);

            // Au lieu de DeleteOneAsync, on fait UpdateOneAsync
            UpdateDefinition<Comment> update = Builders<Comment>.Update
                .Set(c => c.Content, "[Deleted]")
                .Set(c => c.UserId, "Deleted")
                .Set(c => c.IsEdited, false) // Techniquement c'est supprimé, pas édité
                .Set(c => c.IsDeleted, true); // Il te faudrait ajouter ce champ bool dans l'entité

            await mongoContext.Comments.UpdateOneAsync(commentFilter, update, cancellationToken: cancellationToken);
            // On supprime quand même les réactions
            await mongoContext.CommentReactions.DeleteManyAsync(r => r.CommentId == command.CommentId, cancellationToken);

            // Valider la transaction Mongo
            await session.CommitTransactionAsync(cancellationToken);

            // 4. Mise à jour du compteur dans Postgres (Hors transaction Mongo, mais séquentiel)
            await statsService.DecrementCommentCountAsync(Guid.Parse(comment.MediaId), cancellationToken);

            return Result.Success();
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync(cancellationToken);
            throw;
        }
    }
}
