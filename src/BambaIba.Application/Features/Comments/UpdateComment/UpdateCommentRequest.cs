namespace BambaIba.Application.Features.Comments.UpdateComment;

public sealed record UpdateCommentRequest(Guid VideoId, Guid CommentId, string Content);
