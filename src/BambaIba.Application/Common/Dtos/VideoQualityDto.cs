namespace BambaIba.Application.Common.Dtos;
public record VideoQualityDto
{
    public string Quality { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
}
