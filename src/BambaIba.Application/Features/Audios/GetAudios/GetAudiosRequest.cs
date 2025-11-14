
namespace BambaIba.Application.Features.Audios.GetAudios;
public sealed record GetAudiosRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Genre { get; init; }
    public string? Search { get; init; }
}
