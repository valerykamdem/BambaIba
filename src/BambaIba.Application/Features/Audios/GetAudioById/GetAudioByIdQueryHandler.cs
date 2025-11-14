using BambaIba.Application.Features.Videos.GetVideoById;
using BambaIba.Domain.Audios;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Videos;
using Cortex.Mediator.Queries;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Audios.GetAudioById;
public sealed class GetAudioByIdQueryHandler : IQueryHandler<GetAudioByIdQuery, Result<AudioDetailResult>>
{
    private readonly IAudioRepository _audioRepository;
    private readonly ILogger<GetAudioByIdQueryHandler> _logger;

    public GetAudioByIdQueryHandler(
        IAudioRepository audioRepository,
        ILogger<GetAudioByIdQueryHandler> logger)
    {
        _audioRepository = audioRepository;
        _logger = logger;
    }

    public async Task<Result<AudioDetailResult>> Handle(GetAudioByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving audio with ID: {AudioId}", query.AudioId);
            Audio audio = await _audioRepository.GetByIdAsync(query.AudioId, cancellationToken);
            if (audio == null)
            {
                _logger.LogWarning("Audio with ID: {AudioId} not found", query.AudioId);
                return null;
            }

            _logger.LogInformation("Successfully retrieved audio with ID: {AUdioId}", query.AudioId);

            return Result.Success(new AudioDetailResult
            {
                Id = audio.Id,
                Title = audio.Title,
                Description = audio.Description,
                AudioUrl = audio.StoragePath,
                CoverUrl = audio.CoverImagePath,
                Duration = audio.Duration,
                PlayCount = audio.PlayCount,
                LikeCount = audio.LikeCount,
                DislikeCount = audio.DislikeCount,
                CreatedAt = audio.CreatedAt,
                UserId = audio.UserId,
                CommentCount = audio.CommentCount

            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audio with ID: {AudioId}", query.AudioId);
            throw;
        }
    }
}
