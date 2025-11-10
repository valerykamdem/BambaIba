namespace BambaIba.Application.Features.Comments.CreateComment;

public sealed record CreateCommentRequest(
    Guid VideoId, 
    string Content,
    Guid? ParentCommentId);
