using BambaIba.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BambaIba.Domain.Entities.Mongo.Comments;

public class CommentReaction
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("commentId")]
    public string CommentId { get; set; }

    [BsonElement("userId")]
    public string UserId { get; set; }

    [BsonElement("type")] // Like, Dislike
    public ReactionType Type { get; set; } // "like" ou "dislike"

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
