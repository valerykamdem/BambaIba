using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BambaIba.Domain.Entities.Mongo.Comments;

public class Comment
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    [BsonElement("mediaId")]
    public string MediaId { get; set; }

    [BsonElement("userId")]
    public string UserId { get; set; }

    [BsonElement("content")]
    public string Content { get; set; }

    // --- Gestion de l'édition ---
    [BsonElement("isEdited")]
    public bool IsEdited { get; set; } = false;
    
    [BsonElement("isDeleted")]
    public bool IsDeleted { get; set; } = false;

    [BsonElement("editedAt")]
    public DateTime? EditedAt { get; set; }

    // --- Gestion des Réponses (Structure Plate) ---
    [BsonElement("parentId")]
    public string? ParentId { get; set; } // Si null, c'est un commentaire principal

    // --- Engagement ---
    [BsonElement("likeCount")]
    public int LikeCount { get; set; } = 0;

    [BsonElement("dislikeCount")]
    public int DislikeCount { get; set; } = 0;

    [BsonElement("repliesCount")]
    public int RepliesCount { get; set; } = 0; // Valeur par défaut

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
