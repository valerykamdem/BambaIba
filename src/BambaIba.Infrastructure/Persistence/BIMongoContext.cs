using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Mongo.Comments;
using BambaIba.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Spectre.Console;


namespace BambaIba.Infrastructure.Persistence;


public class BIMongoContext : IBIMongoContext
{
    private readonly IMongoDatabase _database;

    // On expose directement les collections comme des propriétés publiques
    public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("comments");
    public IMongoCollection<CommentReaction> CommentReactions => _database.GetCollection<CommentReaction>("commentReactions");

    public BIMongoContext(IOptions<MongoSettings> settings, IMongoClient mongoClient)
    {
        _database = mongoClient.GetDatabase(settings.Value.Database);

        // On initialise les index au démarrage de l'application (Singleton)
        InitializeIndexes().GetAwaiter().GetResult();
    }

    private async Task InitializeIndexes()
    {
        // Index sur MediaId pour charger les commentaires d'une vidéo vite
        await Comments.Indexes.CreateOneAsync(
            new CreateIndexModel<Comment>(
                Builders<Comment>.IndexKeys.Ascending(x => x.MediaId)
                    .Ascending(x => x.CreatedAt) // Composé pour trier par date
            ));

        // Index sur UserId pour voir l'historique d'un utilisateur
        await Comments.Indexes.CreateOneAsync(
            new CreateIndexModel<Comment>(
                Builders<Comment>.IndexKeys.Ascending(x => x.UserId)
            ));

        // Index sur ParentId pour les réponses
        await Comments.Indexes.CreateOneAsync(
            new CreateIndexModel<Comment>(
                Builders<Comment>.IndexKeys.Ascending(x => x.ParentId)
            ));

        await CommentReactions.Indexes.CreateOneAsync(new CreateIndexModel<CommentReaction>(
            Builders<CommentReaction>.IndexKeys
                .Ascending(x => x.CommentId)
                .Ascending(x => x.UserId),
            new CreateIndexOptions { Unique = true } // Garantit l'unicité (Sécurité DB)
        ));
    }


    // --- IMPLÉMENTATION GET REPLIES (AVEC CURSEUR) ---
    public async Task<CursorPagedResult<CommentDto>> GetRepliesAsync(
    string parentId,
    string? cursor,
    int limit,
    string? currentUserId,
    CancellationToken ct)
    {
        // 1. Construction du filtre de base (ParentId)
        FilterDefinition<Comment> filter = Builders<Comment>.Filter.Eq(x => x.ParentId, parentId);

        // 2. Application du Curseur (Pagination)
        if (!string.IsNullOrEmpty(cursor))
        {
            CursorData cursorData = Application.Abstractions.Dtos.CursorExtensions.Decode<CursorData>(cursor);
            if (cursorData != null)
            {
                // Logique : On cherche ce qui est STRICTEMENT après le curseur
                DateTime cursorDate = cursorData.CreatedAt;
                string cursorId = cursorData.Id.ToString(); // Convertir Guid en string pour comparer avec l'ID String Mongo

                filter &= Builders<Comment>.Filter.Or(
                    // Date plus récente
                    Builders<Comment>.Filter.Lt(x => x.CreatedAt, cursorDate),
                    // Même date, mais ID plus petit (car on trie par ID descendant pour l'unicité)
                    Builders<Comment>.Filter.And(
                        Builders<Comment>.Filter.Eq(x => x.CreatedAt, cursorDate),
                        Builders<Comment>.Filter.Lt(x => x.Id, cursorId)
                    )
                );
            }
        }

        // 3. Pipeline d'Aggregation
        // On fait tout en une seule requête : Filtrer -> Trier -> Limiter -> Calculer le nombre de réponses
        var pipeline = new BsonDocument[]
        {
        new("$match", filter.ToBsonDocument()),
        new("$sort", new BsonDocument("createdAt", -1).Add("_id", -1)),
        new("$limit", limit + 1), // On prend +1 pour savoir si y a une suite
        new("$lookup", new BsonDocument
        {
            { "from", "comments" },
            { "let", new BsonDocument("parentId", "$_id") },
            { "pipeline", new BsonArray
                {
                    new BsonDocument("$match", new BsonDocument("$expr", new BsonDocument("$eq", new BsonArray { "$parentId", "$$parentId" })))
                }
            },
            { "as", "repliesArray" }
        }),
        new("$addFields", new BsonDocument("repliesCount", new BsonDocument("$size", "$repliesArray")))
        };

        // Exécution de l'aggregation
        //var mongoCursor = await Comments.Aggregate<BsonDocument>(pipeline, cancellationToken: ct).ToCursor(ct);
        //var rawDocs = await mongoCursor.ToListAsync(ct); // On matérialise tout pour compter
        List<BsonDocument> rawDocs = await Comments.Aggregate<BsonDocument>(pipeline, cancellationToken: ct).ToListAsync(ct);

        // 4. Détermination de la pagination
        bool hasNextPage = rawDocs.Count > limit;
        var itemsToProcess = rawDocs.Take(limit).ToList();

        // 5. Optimisation RÉACTIONS (IsLiked)
        // Au lieu de faire une requête par commentaire, on récupère TOUTES les réactions de l'utilisateur
        // pour la liste de commentaires récupérée.
        HashSet<string> likedCommentIds = [];
        if (!string.IsNullOrEmpty(currentUserId) && itemsToProcess.Any())
        {
            var commentIds = itemsToProcess.Select(d => d["_id"].AsString).ToList();
            List<string> userReactions = await CommentReactions
                .Find(r => r.UserId == currentUserId && commentIds.Contains(r.CommentId))
                .Project(r => r.CommentId)
                .ToListAsync(ct);

            foreach (string id in userReactions)
                likedCommentIds.Add(id);
        }

        // 6. Mapping vers DTO
        var resultItems = new List<CommentDto>();
        foreach (BsonDocument? doc in itemsToProcess)
        {
            // Extraction des champs Bson
            string id = doc["_id"].AsString;
            string mediaId = doc["mediaId"].AsString;
            string userId = doc["userId"].AsString;
            string content = doc["content"].AsString;
            DateTime createdAt = doc["createdAt"].ToUniversalTime();
            bool isEdited = doc.Contains("isEdited") && doc["isEdited"].AsBoolean;
            bool isLiked = doc.Contains("isLiked") && doc["isLiked"].AsBoolean;
            int likeCount = doc.Contains("likeCount") ? doc["likeCount"].AsInt32 : 0;
            int dislikeCount = doc.Contains("dislikeCount") ? doc["dislikeCount"].AsInt32 : 0;
            int repliesCount = doc.Contains("repliesCount") ? doc["repliesCount"].AsInt32 : 0;

            resultItems.Add(new CommentDto(
                id,
                mediaId,
                userId,
                "UserName Placeholder", // TODO: Remplacer par lookup utilisateur
                null, // Avatar Placeholder
                content,
                createdAt,
                likeCount,
                dislikeCount,
                repliesCount,
                isEdited,
                likedCommentIds.Contains(id) // Vérification via le HashSet optimisé
            ));
        }

        // 7. Génération du curseur suivant
        string? nextCursor = null;
        if (hasNextPage && resultItems.Any())
        {
            CommentDto lastItem = resultItems.Last();
            nextCursor = Application.Abstractions.Dtos.CursorExtensions.Encode(new CursorData(lastItem.CreatedAt, Guid.Parse(lastItem.Id)));
        }

        return new CursorPagedResult<CommentDto>
        (
            resultItems,
            nextCursor,
            hasNextPage            
        );
    }
}
