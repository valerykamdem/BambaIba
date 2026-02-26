using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Playlists;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Playlists.CreatePlaylist;

public sealed record CreatePlaylistCommand(string Name, string? Description, bool IsPublic = false);

public sealed class CreatePlaylistHandler(
    IBIDbContext dbContext,
    IUserContextService _userContextService,
    ILogger<CreatePlaylistHandler> logger)
{

    public async Task<Result<Guid>> Handle(CreatePlaylistCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await _userContextService
               .GetCurrentContext();

            if (userContext == null)
                return Result.Failure<Guid>(Error.Unauthorized("401", "User not authenticated"));


            var playlist = new Playlist
            {
                UserId = userContext.LocalUserId,
                Name = command.Name,
                Description = command.Description!,
                Visibility = command.IsPublic ? PlaylistVisibility.Public : PlaylistVisibility.Private
            };

            await dbContext.Playlists.AddAsync(playlist, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Playlist created: {PlaylistId}", playlist.Id);

            return Result.Success(playlist.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating playlist");
            return Result.Failure<Guid>(Error.Failure("An error occurred", ex.Message.ToString()));
        }
    }
}
