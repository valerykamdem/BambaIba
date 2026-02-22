using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Domain.Entities.Mongo.Comments;
using MongoDB.Driver;

namespace BambaIba.Application.Abstractions.Interfaces;

public interface IBIMongoContext
{
    IMongoCollection<Comment> Comments { get; }
    IMongoCollection<CommentReaction> CommentReactions { get; }

    // Helper Méthode for the responses
    Task<CursorPagedResult<CommentDto>> GetRepliesAsync(
        string parentId, 
        string? cursor, 
        int limit, 
        string? currentUserId, 
        CancellationToken ct);
}
