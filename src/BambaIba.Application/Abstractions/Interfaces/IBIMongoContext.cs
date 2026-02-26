using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Domain.Entities.Mongo.Comments;
using BambaIba.Domain.Entities.Mongo.LiveChatMessages;
using BambaIba.Domain.Entities.Mongo.MediaProgresses;
using MongoDB.Driver;

namespace BambaIba.Application.Abstractions.Interfaces;

public interface IBIMongoContext
{
    IMongoCollection<Comment> Comments { get; }
    IMongoCollection<CommentReaction> CommentReactions { get; }
    IMongoCollection<MediaProgress> MediaProgresses { get; }
    IMongoCollection<LiveChatMessage> LiveChatMessages { get; }

    // Helper Méthode for the responses
    Task<CursorPagedResult<CommentDto>> GetRepliesAsync(
        string parentId, 
        string? cursor, 
        int limit, 
        string? currentUserId, 
        CancellationToken ct);
}
