using BambaIba.Application.Extensions;
using BambaIba.Domain.Audios;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Audios.GetAudios;
public class GetAudiosQueryHandler : IQueryHandler<GetAudiosQuery, Result<GetAudiosResult>>
{
    private readonly IAudioRepository _audioRepository;
    private readonly ILogger<GetAudiosQueryHandler> _logger;

    public GetAudiosQueryHandler(
        IAudioRepository audioRepository,
        ILogger<GetAudiosQueryHandler> logger)
    {
        _audioRepository = audioRepository;
        _logger = logger;
    }

    public async Task<Result<GetAudiosResult>> Handle(
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
        //query = query.ApplySorting(request.SortBy, request.SortDescending);

        int totalCount = await audios.CountAsync(cancellationToken);

        _logger.LogInformation("Total={Total}", totalCount);

        PagedResult<AudioDto> pagedResult = await audios
            .Select(a => new AudioDto
            {
                Id = a.Id,
                Title = a.Title ?? string.Empty,
                Artist = a.Artist ?? string.Empty,
                Album = a.Album ?? string.Empty,
                Genre = a.Genre ?? string.Empty,
                CoverImageUrl = a.CoverImagePath,
                Duration = a.Duration,
                PlayCount = a.PlayCount,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        return Result.Success(new GetAudiosResult
        {
            Audios = pagedResult.Items,
            TotalCount = totalCount
        });
    }
}
