using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities.Mongo.Comments;

public static class CommentErrors
{
    public static Error NotFound(Guid commentId) => Error.NotFound(
        "Comments.NotFound",
        $"The comment with the Id = '{commentId}' was not found");

    public static readonly Error NotFoundParent = Error.NotFound(
        "Comments.NotFoundParent",
        "Parent comment was not found");

    //public static readonly Error ErrorCreating = Error.Conflict(
    //    "Comment.CommentErrorCreating",
    //    "Error creating comment");

    public static Error ErrorCreating(string message) => Error.Conflict(
        "Comment.CommentErrorCreating",
        $"Error creating comment '{message}'");
}
