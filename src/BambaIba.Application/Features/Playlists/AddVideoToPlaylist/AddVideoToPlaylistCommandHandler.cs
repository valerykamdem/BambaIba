using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Playlists;
using BambaIba.Domain.PlaylistVideos;
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
    private readonly IPlaylistVideoRepository _playlistVideosRepository;
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AddVideoToPlaylistCommandHandler> _logger;

    public AddVideoToPlaylistCommandHandler(
        IPlaylistRepository playlistRepository,
        IPlaylistVideoRepository playlistVideosRepository,
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

            Video video = await _videoRepository.GetVideoById(command.VideoId);

            if (video == null)
                return AddVideoToPlaylistResult.Failure("Video not found");

            // Vérifier si déjà dans la playlist
            bool exists = playlist.Videos.Any(pv => pv.VideoId == command.VideoId);
            if (exists)
                return AddVideoToPlaylistResult.Failure("Video already in playlist");

            // Position = dernier + 1
            int maxPosition = playlist.Videos.Any()
                ? playlist.Videos.Max(pv => pv.Position)
                : 0;

            var playlistVideo = new PlaylistVideo
            {
                PlaylistId = command.PlaylistId,
                VideoId = command.VideoId,
                Position = maxPosition + 1,
                AddedAt = DateTime.UtcNow
            };

            await _playlistVideosRepository.AddAsync(playlistVideo);
            playlist.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Video {VideoId} added to playlist {PlaylistId}",
                command.VideoId, command.PlaylistId);

            return AddVideoToPlaylistResult.Success($"Video {command.VideoId} added to playlist {command.PlaylistId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding video to playlist");
            return AddVideoToPlaylistResult.Failure("An error occurred");
        }
    }
}
