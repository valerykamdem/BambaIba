namespace BambaIba.Application.Abstractions.Dtos;

public sealed record CommentDto(
    string Id,
    string MediaId,
    string UserId,
    string UserName,      // TODO: Remplir via lookup User
    string? UserAvatar,   // TODO: Remplir via lookup User
    string Content,
    DateTime CreatedAt,
    int LikeCount,
    int DislikeCount,    
    int RepliesCount,     // Nouveau : Calculé via Aggregationb
    bool IsEdited,
    bool IsLiked          // Nouveau : Calculé via lookup Réactions
);
