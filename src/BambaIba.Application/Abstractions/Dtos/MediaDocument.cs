using BambaIba.Application.Abstractions.Interfaces;

namespace BambaIba.Application.Abstractions.Dtos;

public sealed record MediaDocument
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Speaker { get; init; }
    public string? Category { get; init; }
    public List<string> Tags { get; init; } = [];
    public string MediaType { get; init; } // "video" ou "audio"
    public DateTime PublishedAt { get; init; }
}
