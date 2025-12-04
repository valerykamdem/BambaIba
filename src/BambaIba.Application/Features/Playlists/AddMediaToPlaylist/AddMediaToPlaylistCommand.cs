using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.Playlists.AddMediaToPlaylist;
public record AddMediaToPlaylistCommand : ICommand<Result<AddMediaToPlaylistResult>>
{
    public Guid PlaylistId { get; init; }
    public Guid MediaId { get; init; }
}

public record AddMediaToPlaylistResult
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public string? ErrorMessage { get; init; }

    public static AddMediaToPlaylistResult Success(string message) => 
        new() { IsSuccess = true, Message = message };
    public static AddMediaToPlaylistResult Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}
