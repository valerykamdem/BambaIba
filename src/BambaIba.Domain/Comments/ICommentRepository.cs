using BambaIba.SharedKernel.Comments;

namespace BambaIba.Domain.Comments;
public interface ICommentRepository
{
    Task AddCommentAsync(Comment comment);
    void DeleteComment(Comment comment);
    Task<Comment> GetComment(Guid id);
    Task<GetCommentsResult> GetComments(Guid videoId, 
        int page, int pageSize, CancellationToken cancellationToken);
    Task UpdateComment(Comment comment, CancellationToken cancellationToken);
    Task<bool> GetParentComment(Guid ParentCommentId, 
        CancellationToken cancellationToken= default);
    Task<GetRepliesResult> GetReplies(Guid videoId, 
        Guid parentCommentId, CancellationToken cancellationToken);
}
