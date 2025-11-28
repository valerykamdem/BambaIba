using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;

namespace BambaIba.Application.Features.MediaBase.GetMediaById;

public record GetMediaByIdQuery : IQuery<Result<MediaWithQualitiesResult>>
{
    public Guid MediaId { get; init; }
}
