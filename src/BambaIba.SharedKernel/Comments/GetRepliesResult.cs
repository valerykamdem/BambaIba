
namespace BambaIba.SharedKernel.Comments;
public sealed record GetRepliesResult
{
    public List<CommentDto> Replies { get; init; } = [];
    public int TotalCount { get; init; }
}
