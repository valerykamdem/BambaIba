using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Mongo.Comments;
using BambaIba.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BambaIba.Application.Features.Comments.GetComments;

public sealed record GetCommentsQuery(
    Guid MediaId,
    string? Cursor,
    int PageSize = 10
);

public sealed class GetCommentsHandler(
    IUserContextService userContextService,
    IBIMongoContext mongoContext,
    ILogger<GetCommentsHandler> logger)
{

    public async Task<Result<CursorPagedResult<CommentDto>>> Handle(
        GetCommentsQuery query,
        CancellationToken cancellationToken)
    {
        try
        {

            // 1. Récupération de l'utilisateur courant
            UserContext userContext = await userContextService
                .GetCurrentContext();
            string? currentUserId = userContext?.LocalUserId.ToString();

            // 2. Filtre : Commentaires Racines
            // CORRECTION ICI : Utiliser .IsNull() pour vérifier le null
            FilterDefinition<Comment> filter = Builders<Comment>.Filter.And(
                Builders<Comment>.Filter.Eq(x => x.MediaId, query.MediaId.ToString()),
                Builders<Comment>.Filter.Eq(x => x.ParentId, null)//IsNull(x => x.ParentId)
            );

            // 3. Application du Curseur
            if (!string.IsNullOrEmpty(query.Cursor))
            {
                // CORRECTION ICI : Vérification classique de nullité
                CursorData? cursorData = CursorExtensions.Decode<CursorData>(query.Cursor);

                if (cursorData != null) // Au lieu de cursorData.HasValue
                {
                    DateTime cursorDate = cursorData.CreatedAt; // Au lieu de cursorData.Value.CreatedAt
                    string cursorId = cursorData.Id.ToString();

                    filter &= Builders<Comment>.Filter.Or(
                        Builders<Comment>.Filter.Lt(x => x.CreatedAt, cursorDate),
                        Builders<Comment>.Filter.And(
                            Builders<Comment>.Filter.Eq(x => x.CreatedAt, cursorDate),
                            Builders<Comment>.Filter.Lt(x => x.Id, cursorId)
                        )
                    );
                }
            }

            // 4. Requête
            List<Comment> rawItems = await mongoContext.Comments
                .Find(filter)
                .SortByDescending(c => c.CreatedAt)
                .ThenByDescending(c => c.Id)
                .Limit(query.PageSize + 1)
                .ToListAsync(cancellationToken);

            // 5. Pagination
            bool hasNextPage = rawItems.Count > query.PageSize;
            var items = rawItems.Take(query.PageSize).ToList();

            // 6. Optimisation Reactions
            HashSet<string> likedCommentIds = [];
            if (!string.IsNullOrEmpty(currentUserId) && items.Any())
            {
                var commentIds = items.Select(c => c.Id).ToList();
                List<string> userReactions = await mongoContext.CommentReactions
                    .Find(r => r.UserId == currentUserId && commentIds.Contains(r.CommentId))
                    .Project(r => r.CommentId)
                    .ToListAsync(cancellationToken);

                foreach (string id in userReactions)
                    likedCommentIds.Add(id);
            }

            // 7. Mapping
            var resultItems = new List<CommentDto>();
            foreach (Comment? item in items)
            {
                // Ici, item.RepliesCount existe maintenant grâce à la correction de l'entité
                resultItems.Add(new CommentDto(
                    item.Id,
                    item.MediaId,
                    item.UserId,
                    "UserName", // TODO: Remplacer par lookup User
                    null,
                    item.Content,
                    item.CreatedAt,
                    item.LikeCount,
                    item.DislikeCount,
                    item.RepliesCount,
                    item.IsEdited,                   
                    likedCommentIds.Contains(item.Id)
                ));
            }

            // 8. Génération Curseur
            string? nextCursor = null;
            if (hasNextPage && resultItems.Any())
            {
                CommentDto last = resultItems.Last();
                nextCursor = CursorExtensions.Encode(new CursorData(last.CreatedAt, Guid.Parse(last.Id)));
            }

            return Result.Success(new CursorPagedResult<CommentDto>
            (
                resultItems,
                nextCursor,
                hasNextPage
                
            ));



            //FilterDefinition<Comment> filter = Builders<Comment>.Filter.Eq(x => x.MediaId, query.MediaId.ToString()) &
            //         Builders<Comment>.Filter.Eq(x => x.ParentId, null);

            //var comments = await mongoContext.Comments
            //.Find(filter)
            //.SortByDescending(c => c.CreatedAt)
            //.Limit(10)
            //.Project(c => new CommentDto(
            //    c.Id,
            //    c.MediaId,
            //    c.UserId,
            //    "User Name", // TODO: Lookup user
            //    null,
            //    c.Content,
            //    c.CreatedAt,
            //    c.LikeCount,
            //    c.DislikeCount,
            //    c.re
            //    c.IsEdited
            //))
            //.ToListAsync(cancellationToken);

            //return Result.Success(comments);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting comments");
            throw;
        }
    }
}
