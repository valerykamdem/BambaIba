namespace BambaIba.Application.Abstractions.Dtos;

public sealed record CursorPagedResult<T>(
    IReadOnlyList<T> Items,
    string? NextCursor,  // null si dernière page
    bool HasNextPage);
