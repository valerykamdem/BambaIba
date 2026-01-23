using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Playlists;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Playlists.GetPlaylistById;
public class GetPlaylistByIdQueryHandler : IQueryHandler<GetPlaylistByIdQuery, Result<PlaylistDetailResult?>>
{
    private readonly IPlaylistRepository _playlistRepository;
    private readonly IUserContextService _userContextService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GetPlaylistByIdQueryHandler> _logger;

    public GetPlaylistByIdQueryHandler(
        IPlaylistRepository playlistRepository,
        IUserContextService userContextService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<GetPlaylistByIdQueryHandler> logger)
    {
        _playlistRepository = playlistRepository;
        _userContextService = userContextService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<Result<PlaylistDetailResult>> Handle(
        GetPlaylistByIdQuery query,
        CancellationToken cancellationToken)
    {
        UserContext userContext = await _userContextService
            .GetCurrentContext(_httpContextAccessor.HttpContext);

        Playlist? playlist = await _playlistRepository
            .GetByIdAsync(query.PlaylistId, cancellationToken);

        if (playlist == null)
            return null;

        // Vérifier les permissions
        if (!playlist.IsPublic && playlist.UserId != userContext.LocalUserId)
            return null;

        var videos = playlist.Items
            .OrderBy(pv => pv.Position)
            .Select(pv => new PlaylistItemDto
            {
                MediaId = pv.MediaId,
                Title = pv.Media.Title,
                ThumbnailUrl = pv.Media.ThumbnailPath,
                Duration = pv.Media.Duration,
                Position = pv.Position,
                AddedAt = pv.AddedAt
            })
            .ToList();

        return Result.Success(new PlaylistDetailResult
        {
            Id = playlist.Id,
            UserId = playlist.UserId,
            Name = playlist.Name,
            Description = playlist.Description,
            IsPublic = playlist.IsPublic,
            MediaCount = videos.Count,
            CreatedAt = playlist.CreatedAt,
            UpdatedAt = playlist.UpdatedAt,
            Videos = videos
        });
    }
}
