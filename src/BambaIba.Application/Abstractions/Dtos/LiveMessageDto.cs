
namespace BambaIba.Application.Abstractions.Dtos;

public sealed record LiveMessageDto(
    Guid UserId,
    string Username,
    string Content,
    DateTime SentAt
);
