using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.Videos.GetVideoById;

public record GetVideoByIdQuery : IQuery<Result<VideoWithQualitiesResult>>
{
    public Guid VideoId { get; init; }
}
