namespace BambaIba.Application.Abstractions.Dtos;

public sealed record PlaylistDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    int MediaCount,
    string? ThumbnailUrl,
    List<MediaDto> Medias
);
