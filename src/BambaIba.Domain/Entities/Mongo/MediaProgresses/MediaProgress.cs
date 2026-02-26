using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BambaIba.Domain.Entities.Mongo.MediaProgresses;

public class MediaProgress
{
    [BsonId]
    // Format : "UserId_MediaId" (ex: "uuid1_uuid2")
    // Cela évite d'avoir besoin d'un index composé complexe
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("u")] // "u" pour User (shorthand)
    public string UserId { get; set; } = string.Empty;

    [BsonElement("m")] // "m" pour Media
    public string MediaId { get; set; } = string.Empty;

    [BsonElement("p")] // "p" pour Position (en secondes)
    public int PositionSeconds { get; set; } = 0;

    [BsonElement("c")] // "c" pour Completed
    public bool IsCompleted { get; set; } = false;

    [BsonElement("lu")] // "lu" pour LastUpdated
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
