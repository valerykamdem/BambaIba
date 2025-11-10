namespace BambaIba.Application.Abstractions.Dtos;

public sealed record AuthResultDto(string UserId, string Email, List<string> Roles,
    string Access_Token, string Refresh_Token);
