using BambaIba.Application.Extensions;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.MediaBase.GetMedia;

public sealed record GetMediaQuery(
    int Page,
    int PageSize,
    string? Search) : IQuery<Result<PagedResult<MediaDto>>>;



