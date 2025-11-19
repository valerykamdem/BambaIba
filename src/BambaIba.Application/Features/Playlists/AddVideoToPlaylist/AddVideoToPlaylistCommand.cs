using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.Playlists.AddVideoToPlaylist;
public record AddVideoToPlaylistCommand : ICommand<Result<AddVideoToPlaylistResult>>
{
    public Guid PlaylistId { get; init; }
    public Guid MediaId { get; init; }
}

public record AddVideoToPlaylistResult
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public string? ErrorMessage { get; init; }

    public static AddVideoToPlaylistResult Success(string message) => 
        new() { IsSuccess = true, Message = message };
    public static AddVideoToPlaylistResult Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}
