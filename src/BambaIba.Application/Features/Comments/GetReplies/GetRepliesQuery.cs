using BambaIba.Application.Abstractions.Dtos;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Comments;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.Comments.GetReplies;
public sealed record GetRepliesQuery : IQuery<Result<GetRepliesResult>>
{
    public Guid VideoId { get; init; }
    public Guid ParentCommentId { get; init; }
}
