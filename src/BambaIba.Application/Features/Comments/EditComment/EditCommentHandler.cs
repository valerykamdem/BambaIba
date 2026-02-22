using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Mongo.Comments;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Comments;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace BambaIba.Application.Features.Comments.EditComment;

public sealed record EditCommentCommand(
    string CommentId, // ID MongoDB (String)
    string NewContent,
    Guid CurrentUserId
);

public sealed class EditCommentHandler(
    IUserContextService userContextService,
    IHttpContextAccessor httpContextAccessor,
    IBIMongoContext mongoContext,
    ILogger<EditCommentHandler> logger)
{

    public async Task<Result> Handle(EditCommentCommand command, CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await userContextService
                .GetCurrentContext(httpContextAccessor.HttpContext);     

            // 1. Vérification : Le commentaire appartient-il à l'utilisateur ?
            FilterDefinition<Comment> filter = Builders<Comment>.Filter.And(
                Builders<Comment>.Filter.Eq(c => c.Id, command.CommentId),
                Builders<Comment>.Filter.Eq(c => c.UserId, userContext.LocalUserId.ToString())
            );

            // 2. Définition de la mise à jour (SetOnInsert si besoin, Update si existe)
            UpdateDefinition<Comment> update = Builders<Comment>.Update
                .Set(c => c.Content, command.NewContent)
                .Set(c => c.IsEdited, true)
                .Set(c => c.EditedAt, DateTime.UtcNow);
            //.Set(c => c.UpdatedAt, DateTime.UtcNow); // Si tu as un champ UpdatedAt

            UpdateResult result = await mongoContext.Comments.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

            if (result.ModifiedCount == 0)
            {
                return Result.Failure(Error.Problem("400", "Not Found")); // Soit introuvable, soit pas le propriétaire
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating comment");
            return Result.Failure(Error.Problem("400", "An error occurred"));
        }
    }
}
