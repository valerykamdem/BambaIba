using BambaIba.SharedKernel.Comments;

namespace BambaIba.Domain.Comments;
public interface ICommentRepository
{
    Task AddCommentAsync(Comment comment);
    void DeleteComment(Comment comment);
    Task<Comment> GetComment(Guid id);
    IQueryable<Comment> GetComments(Guid videoId, CancellationToken cancellationToken);
    Task UpdateComment(Comment comment, CancellationToken cancellationToken);
    Task<bool> GetParentComment(Guid ParentCommentId, 
        CancellationToken cancellationToken= default);
    IQueryable<Comment> GetReplies(Guid videoId, 
        Guid parentCommentId, CancellationToken cancellationToken);
}
