using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Videos;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.Videos.GetVideoById;

public record GetVideoByIdQuery : IQuery<Result<VideoDetailResult>>
{
    public Guid VideoId { get; init; }
}
