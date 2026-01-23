using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BambaIba.Domain.Entities.MediaBase;
using BambaIba.Domain.Entities.PlaylistItems;
using BambaIba.Domain.Entities.Playlists;

namespace BambaIba.Application.Features.Playlists.AddMediaToPlaylist;
public class AddMediaToPlaylistCommandHandler : 
    ICommandHandler<AddMediaToPlaylistCommand, Result<AddMediaToPlaylistResult>>
{
    private readonly IPlaylistRepository _playlistRepository;
    private readonly IPlaylistItemRepository _playlistMediasRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AddMediaToPlaylistCommandHandler> _logger;

    public AddMediaToPlaylistCommandHandler(
        IPlaylistRepository playlistRepository,
        IPlaylistItemRepository playlistMediasRepository,
        IMediaRepository mediaRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AddMediaToPlaylistCommandHandler> logger)
    {
        _playlistRepository = playlistRepository;
        _playlistMediasRepository = playlistMediasRepository;
        _mediaRepository = mediaRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<Result<AddMediaToPlaylistResult>> Handle(
        AddMediaToPlaylistCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await _userContextService
                .GetCurrentContext(_httpContextAccessor.HttpContext);

            Playlist? playlist = await _playlistRepository.GetByIdAsync(command.PlaylistId, cancellationToken);

            if (playlist == null)
                return AddMediaToPlaylistResult.Failure("Playlist not found");

            if (playlist.UserId != userContext.LocalUserId)
                return AddMediaToPlaylistResult.Failure("Unauthorized");

            Media media = await _mediaRepository.GetMediaByIdAsync(command.MediaId, cancellationToken);

            if (media == null)
                return AddMediaToPlaylistResult.Failure("media not found");

            // Vérifier si déjà dans la playlist
            bool exists = playlist.Items.Any(pv => pv.MediaId == command.MediaId);
            if (exists)
                return AddMediaToPlaylistResult.Failure("Media already in playlist");

            // Position = dernier + 1
            int maxPosition = playlist.Items.Any()
                ? playlist.Items.Max(pv => pv.Position)
                : 0;

            var playlistMedia = new PlaylistItem
            {
                PlaylistId = command.PlaylistId,
                MediaId = command.MediaId,
                Position = maxPosition + 1,
                AddedAt = DateTime.UtcNow
            };

            await _playlistMediasRepository.AddAsync(playlistMedia);
            playlist.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Media {MediaId} added to playlist {PlaylistId}",
                command.MediaId, command.PlaylistId);

            return AddMediaToPlaylistResult.Success($"Media {command.MediaId} added to playlist {command.PlaylistId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding Media to playlist");
            return AddMediaToPlaylistResult.Failure("An error occurred");
        }
    }
}
