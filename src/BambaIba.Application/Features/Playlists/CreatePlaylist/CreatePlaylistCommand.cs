using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.Playlists.CreatePlaylist;
public sealed record CreatePlaylistCommand : ICommand<Result<CreatePlaylistResult>>
{
    //public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsPublic { get; init; } = true;
}

public sealed record CreatePlaylistResult
{
    public bool IsSuccess { get; init; }
    public Guid PlaylistId { get; init; }
    public string? ErrorMessage { get; init; }

    public static CreatePlaylistResult Success(Guid playlistId)
        => new() { IsSuccess = true, PlaylistId = playlistId };

    public static CreatePlaylistResult Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}
