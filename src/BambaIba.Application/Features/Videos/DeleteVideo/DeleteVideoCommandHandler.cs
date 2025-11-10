using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Users;
using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Videos;
using Cortex.Mediator.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Videos.DeleteVideo;
public sealed class DeleteVideoCommandHandler : ICommandHandler<DeleteVideoCommand, Result<DeleteVideoResult>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly IVideoStorageService _storageService;
    private readonly IUserContextService _userContextService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DeleteVideoCommandHandler> _logger;

    public DeleteVideoCommandHandler(
        IVideoRepository videoRepository,
        IVideoStorageService storageService,
        IUserContextService userContextService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DeleteVideoCommandHandler> logger)
    {
        _videoRepository = videoRepository;
        _storageService = storageService;
        _userContextService = userContextService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<Result<DeleteVideoResult>> Handle(DeleteVideoCommand command, CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await _userContextService.GetCurrentContext(_httpContextAccessor.HttpContext);

            Video video = await _videoRepository.GetVideoById(command.VideoId);

            if (video == null)
                return Result.Failure<DeleteVideoResult>(VideoErrors.NotFound(command.VideoId));

           if(video.UserId != userContext.LocalUserId)
                return Result.Failure<DeleteVideoResult>(UserErrors.NotFound(command.VideoId));

            // Supprimer les fichiers associés de stockage
            //await _storageService.DeleteVideoAsync(video.StoragePath);
            await _storageService.DeleteVideoAsync(video.Id.ToString());
            _logger.LogInformation("Deleting video files from storage for VideoId: {VideoId}", video.StoragePath);

            // Supprimer la vidéo de la base de données seulement si l'utilisateur est le propriétaire
            _videoRepository.Delete(video);

            _logger.LogInformation("Deleting video files from storage for VideoId: {VideoId}", command.VideoId);

            return Result.Success(DeleteVideoResult.Success(video.Id));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting video with ID: {VideoId}", command.VideoId);
            throw;
        }

    }
}
