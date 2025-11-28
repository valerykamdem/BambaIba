using BambaIba.SharedKernel.Comments;

namespace BambaIba.Application.Features.MediaBase.DeleteMedia;
public sealed record DeleteMediaResult
{
    public bool IsSuccess { get; init; }
    public Guid MediaId { get; init; }
    public string? ErrorMessage { get; init; }

    public static DeleteMediaResult Success(Guid mediaId) => 
        new() { IsSuccess = true, MediaId = mediaId };
    public static DeleteMediaResult Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}
