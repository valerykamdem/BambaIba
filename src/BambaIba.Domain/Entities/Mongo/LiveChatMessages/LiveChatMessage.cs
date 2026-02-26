using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BambaIba.Domain.Entities.Mongo.LiveChatMessages;

public class LiveChatMessage
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    // The Live Event identifier
    [BsonElement("liveEventId")]
    public Guid LiveEventId { get; set; }

    // User Info
    [BsonElement("userId")]
    public Guid UserId { get; set; }
    [BsonElement("userName")]
    public string Username { get; set; } = string.Empty;

    // Message Content
    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    // Timestamp
    [BsonElement("sentAt")]
    public DateTime SentAt { get; set; }
}
