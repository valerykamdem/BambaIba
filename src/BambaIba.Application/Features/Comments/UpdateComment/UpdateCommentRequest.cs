namespace BambaIba.Application.Features.Comments.UpdateComment;

public sealed record UpdateCommentRequest(Guid MediaId, Guid CommentId, string Content);
