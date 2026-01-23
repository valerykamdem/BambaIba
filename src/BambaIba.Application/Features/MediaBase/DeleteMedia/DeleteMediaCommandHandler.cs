using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.MediaBase;
using BambaIba.Domain.Entities.Users;
using BambaIba.Domain.Entities.Videos;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.MediaBase.DeleteMedia;
public sealed class DeleteMediaCommandHandler : ICommandHandler<DeleteMediaCommand, Result<DeleteMediaResult>>
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IMediaStorageService _storageService;
    private readonly IUserContextService _userContextService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DeleteMediaCommandHandler> _logger;

    public DeleteMediaCommandHandler(
        IMediaRepository mediaRepository,
        IMediaStorageService storageService,
        IUserContextService userContextService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DeleteMediaCommandHandler> logger)
    {
        _mediaRepository = mediaRepository;
        _storageService = storageService;
        _userContextService = userContextService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<Result<DeleteMediaResult>> Handle(DeleteMediaCommand command, CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await _userContextService.GetCurrentContext(_httpContextAccessor.HttpContext);

            Media media = await _mediaRepository.GetMediaByIdAsync(command.MediaId, cancellationToken);

            if (media == null)
                return Result.Failure<DeleteMediaResult>(VideoErrors.NotFound(command.MediaId));

           if(media.UserId != userContext.LocalUserId)
                return Result.Failure<DeleteMediaResult>(UserErrors.NotFound(command.MediaId));

            // Supprimer les fichiers associés de stockage
            await _storageService.DeleteAsync(media.Id.ToString());
            _logger.LogInformation("Deleting video files from storage for VideoId: {VideoId}", media.StoragePath);

            // Supprimer la vidéo de la base de données seulement si l'utilisateur est le propriétaire
            await _mediaRepository.DeleteAsync(media);

            _logger.LogInformation("Deleting video files from storage for VideoId: {VideoId}", command.MediaId);

            return Result.Success(DeleteMediaResult.Success(media.Id));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting video with ID: {VideoId}", command.MediaId);
            throw;
        }

    }
}
