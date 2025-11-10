using BambaIba.Application.Abstractions.Dtos;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;
using Microsoft.AspNetCore.Http;

namespace BambaIba.Application.Features.Videos.Upload;

public sealed record UploadVideoCommand : ICommand<Result<UploadVideoResult>>
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public IFormFile File { get; init; } = null!;
    public string FileName { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string ContentType { get; init; } = string.Empty;
    public List<string>? Tags { get; init; }
}
