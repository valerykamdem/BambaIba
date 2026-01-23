// EndLiveStreamCommandHandler.cs
using BambaIba.Application.Abstractions.Data;
using BambaIba.Domain.Entities.LiveStream;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.LiveStreams.EndLiveStream;

public class EndLiveStreamCommandHandler : ICommandHandler<EndLiveStreamCommand, Result<EndLiveStreamResult>>
{
    private readonly ILiveStreamRepository _liveStreamRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EndLiveStreamCommandHandler> _logger;

    public EndLiveStreamCommandHandler(
        ILiveStreamRepository liveStreamRepository,
        IUnitOfWork unitOfWork,
        ILogger<EndLiveStreamCommandHandler> logger)
    {
        _liveStreamRepository = liveStreamRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<EndLiveStreamResult>> Handle(
        EndLiveStreamCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            LiveStream? stream = await _liveStreamRepository.GetByStreamId(request.StreamId, cancellationToken);

            if (stream == null)
                return EndLiveStreamResult.Failure("Stream not found");

            if (stream.StreamerId != request.StreamerId)
                return EndLiveStreamResult.Failure("Unauthorized");

            stream.Status = LiveStreamStatus.Ended;
            stream.EndedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Live stream ended: {StreamId}", stream.Id);

            return Result.Success(EndLiveStreamResult.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending live stream");
            return EndLiveStreamResult.Failure("An error occurred");
        }
    }
}
