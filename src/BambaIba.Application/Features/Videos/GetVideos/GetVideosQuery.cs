using BambaIba.Application.Extensions;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Comments;
using BambaIba.SharedKernel.Videos;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.Videos.GetVideos;

public sealed record GetVideosQuery(
    int Page,
    int PageSize,
    string? Search) : IQuery<Result<PagedResult<VideoDto>>>;



