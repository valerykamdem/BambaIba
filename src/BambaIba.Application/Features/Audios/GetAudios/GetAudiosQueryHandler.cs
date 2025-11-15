using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Extensions;
using BambaIba.Domain.Audios;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Audios.GetAudios;
public class GetAudiosQueryHandler : IQueryHandler<GetAudiosQuery, Result<PagedResult<AudioDto>>>
{
    private readonly IAudioRepository _audioRepository;
    private readonly IMediaStorageService _mediaStorageService;
    private readonly ILogger<GetAudiosQueryHandler> _logger;

    public GetAudiosQueryHandler(
        IAudioRepository audioRepository,
        IMediaStorageService mediaStorageService,
        ILogger<GetAudiosQueryHandler> logger)
    {
        _audioRepository = audioRepository;
        _mediaStorageService = mediaStorageService;
        _logger = logger;
    }

    public async Task<Result<PagedResult<AudioDto>>> Handle(
        GetAudiosQuery query,
        CancellationToken cancellationToken)
    {

        IQueryable<Audio> audios = _audioRepository.GetAudioAsync();

        if (!string.IsNullOrWhiteSpace(query.Genre))
            audios = audios.Where(a => a.Genre == query.Genre);

        if (!string.IsNullOrWhiteSpace(query.Search))
            audios = audios.Where(a =>
                (a.Title ?? "").Contains(query.Search) ||
                (a.Artist ?? "").Contains(query.Search) ||
                (a.Album ?? "").Contains(query.Search));

        //// Appliquer tri dynamique
        //query = query.ApplySorting(request.SortBy, request.SortDescending)

        PagedResult<AudioDto> pagedResult = await audios
            .Select(a => new AudioDto
            {
                Id = a.Id,
                Title = a.Title ?? string.Empty,
                Artist = a.Artist ?? string.Empty,
                Album = a.Album ?? string.Empty,
                Genre = a.Genre ?? string.Empty,
                CoverImageUrl = _mediaStorageService.GetPublicUrl(BucketType.Image, a.CoverImagePath),
                Duration = a.Duration,
                PlayCount = a.PlayCount,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        return Result.Success(new PagedResult<AudioDto>
        {
            Items = pagedResult.Items,
            TotalCount = pagedResult.TotalCount,
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages
        });
    }
}
