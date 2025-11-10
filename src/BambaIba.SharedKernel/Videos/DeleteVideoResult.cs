
using BambaIba.SharedKernel.Comments;

namespace BambaIba.SharedKernel.Videos;
public sealed record DeleteVideoResult
{
    public bool IsSuccess { get; init; }
    public Guid VideoId { get; init; }
    public string? ErrorMessage { get; init; }

    public static DeleteVideoResult Success(Guid videoId) => 
        new() { IsSuccess = true, VideoId = videoId };
    public static DeleteVideoResult Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}
