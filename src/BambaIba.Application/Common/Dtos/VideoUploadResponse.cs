namespace BambaIba.Application.Common.Dtos;
public record VideoUploadResponse
{
    public Guid VideoId { get; init; }
    public string Status { get; init; } = string.Empty;
}
