// StartLiveStreamCommandHandler.cs
using System.Security.Cryptography;
using BambaIba.Domain.Entities.LiveStream;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.LiveStreams.StartLiveStream;

public class StartLiveStreamCommandHandler : ICommandHandler<StartLiveStreamCommand, Result<StartLiveStreamResult>>
{
    private readonly ILiveStreamRepository _liveStreamRepository;
    private readonly ILogger<StartLiveStreamCommandHandler> _logger;
    private readonly IConfiguration _configuration;

    public StartLiveStreamCommandHandler(
        ILiveStreamRepository liveStreamRepository,
        ILogger<StartLiveStreamCommandHandler> logger,
        IConfiguration configuration)
    {
        _liveStreamRepository = liveStreamRepository;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<Result<StartLiveStreamResult>> Handle(
        StartLiveStreamCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Générer un stream key unique et sécurisé
            string streamKey = GenerateStreamKey();

            var liveStream = new LiveStream
            {
                //Id = Guid.NewGuid(),
                StreamerId = request.StreamerId,
                Title = request.Title,
                Description = request.Description,
                StreamKey = streamKey,
                Status = LiveStreamStatus.Scheduled,
                CreatedAt = DateTime.UtcNow
            };

            await _liveStreamRepository.AddAsync(liveStream);

            string rtmpUrl = $"rtmp://{_configuration["RtmpServer:Host"]}:1935/live/{streamKey}";

            _logger.LogInformation("Live stream created: {StreamId}", liveStream.Id);

            return StartLiveStreamResult.Success(liveStream.Id, streamKey, rtmpUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting live stream");
            return StartLiveStreamResult.Failure("An error occurred");
        }
    }

    private static string GenerateStreamKey()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "")
            .Substring(0, 32);
    }
}
