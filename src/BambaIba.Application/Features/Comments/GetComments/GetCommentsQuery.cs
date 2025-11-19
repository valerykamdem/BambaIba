using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Extensions;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Comments;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.Comments.GetComments;
public sealed record GetCommentsQuery : IQuery<Result<PagedResult<CommentDto>>>
{
    public Guid MediaId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

