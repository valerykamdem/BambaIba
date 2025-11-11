using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Playlists;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Playlists.CreatePlaylist;
public sealed class CreatePlaylistCommandHandler : 
    ICommandHandler<CreatePlaylistCommand, Result<CreatePlaylistResult>>
{
    private readonly IPlaylistRepository _playlistRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CreatePlaylistCommandHandler> _logger;

    public CreatePlaylistCommandHandler(
        IPlaylistRepository playlistRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<CreatePlaylistCommandHandler> logger)
    {
        _playlistRepository = playlistRepository;
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userContextService = userContextService ?? throw new ArgumentNullException(nameof(userContextService));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger;
    }

    public async Task<Result<CreatePlaylistResult>> Handle(CreatePlaylistCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await _userContextService
               .GetCurrentContext(_httpContextAccessor.HttpContext);

            var playlist = new Playlist
            {
                UserId = userContext.LocalUserId,
                Name = command.Name,
                Description = command.Description,
                IsPublic = command.IsPublic,
            };

            await _playlistRepository.AddAsync(playlist);

            _logger.LogInformation("Playlist created: {PlaylistId}", playlist.Id);

            return CreatePlaylistResult.Success(playlist.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating playlist");
            return CreatePlaylistResult.Failure("An error occurred");
        }
    }
}
