using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Extensions;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.Comments.GetReplies;
public sealed record GetRepliesQuery : IQuery<Result<PagedResult<CommentDto>>>
{
    public Guid VideoId { get; init; }
    public Guid ParentCommentId { get; init; }
}
