using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Playlists;
using BambaIba.Domain.PlaylistItems;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Playlists.AddVideoToPlaylist;
public class AddVideoToPlaylistCommandHandler : 
    ICommandHandler<AddVideoToPlaylistCommand, Result<AddVideoToPlaylistResult>>
{
    private readonly IPlaylistRepository _playlistRepository;
    private readonly IPlaylistItemRepository _playlistVideosRepository;
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AddVideoToPlaylistCommandHandler> _logger;

    public AddVideoToPlaylistCommandHandler(
        IPlaylistRepository playlistRepository,
        IPlaylistItemRepository playlistVideosRepository,
        IVideoRepository videoRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AddVideoToPlaylistCommandHandler> logger)
    {
        _playlistRepository = playlistRepository;
        _playlistVideosRepository = playlistVideosRepository;
        _videoRepository = videoRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<Result<AddVideoToPlaylistResult>> Handle(
        AddVideoToPlaylistCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await _userContextService
                .GetCurrentContext(_httpContextAccessor.HttpContext);

            Playlist? playlist = await _playlistRepository.GetByIdAsync(command.PlaylistId, cancellationToken);

            if (playlist == null)
                return AddVideoToPlaylistResult.Failure("Playlist not found");

            if (playlist.UserId != userContext.LocalUserId)
                return AddVideoToPlaylistResult.Failure("Unauthorized");

            Video video = await _videoRepository.GetVideoById(command.MediaId);

            if (video == null)
                return AddVideoToPlaylistResult.Failure("Video not found");

            // Vérifier si déjà dans la playlist
            bool exists = playlist.Items.Any(pv => pv.MediaId == command.MediaId);
            if (exists)
                return AddVideoToPlaylistResult.Failure("Video already in playlist");

            // Position = dernier + 1
            int maxPosition = playlist.Items.Any()
                ? playlist.Items.Max(pv => pv.Position)
                : 0;

            var playlistVideo = new PlaylistItem
            {
                PlaylistId = command.PlaylistId,
                MediaId = command.MediaId,
                Position = maxPosition + 1,
                AddedAt = DateTime.UtcNow
            };

            await _playlistVideosRepository.AddAsync(playlistVideo);
            playlist.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Video {MediaId} added to playlist {PlaylistId}",
                command.MediaId, command.PlaylistId);

            return AddVideoToPlaylistResult.Success($"Media {command.MediaId} added to playlist {command.PlaylistId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding video to playlist");
            return AddVideoToPlaylistResult.Failure("An error occurred");
        }
    }
}
