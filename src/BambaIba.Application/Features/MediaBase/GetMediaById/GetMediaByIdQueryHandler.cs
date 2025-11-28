using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Abstractions.Mappings;
using BambaIba.Domain.Enums;
using BambaIba.Domain.MediaBase;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Videos;
using Cortex.Mediator.Queries;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.MediaBase.GetMediaById;

public sealed class GetMediaByIdQueryHandler : IQueryHandler<GetMediaByIdQuery, Result<MediaWithQualitiesResult>>
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IMediaStorageService _mediaStorageService;
    private readonly ILogger<GetMediaByIdQueryHandler> _logger;

    public GetMediaByIdQueryHandler(
        IMediaRepository mediaRepository,
        IMediaStorageService mediaStorageService,
    ILogger<GetMediaByIdQueryHandler> logger)
    {
        _mediaRepository = mediaRepository;
        _mediaStorageService = mediaStorageService;
        _logger = logger;
    }

    public async Task<Result<MediaWithQualitiesResult>> Handle(GetMediaByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving media with ID: {MediaId}", query.MediaId);
            Media media = await _mediaRepository.GetMediaWithDetailsAsync(query.MediaId, cancellationToken);

            if (media == null)
            {
                _logger.LogWarning("media with ID: {MediaId} not found", query.MediaId);
                return null;
            }

            return Result.Success(MediaMapper.ToResult(media, _mediaStorageService));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Media with ID: {MediaId}", query.MediaId);
            throw;
        }
    }

}
