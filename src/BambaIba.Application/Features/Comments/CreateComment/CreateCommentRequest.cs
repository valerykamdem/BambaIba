namespace BambaIba.Application.Features.Comments.CreateComment;

public sealed record CreateCommentRequest(
    Guid MediaId, 
    string Content,
    Guid? ParentCommentId);
