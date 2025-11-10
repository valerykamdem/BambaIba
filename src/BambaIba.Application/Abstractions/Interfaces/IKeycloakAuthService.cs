
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Features.Register;
using BambaIba.Domain.Users;
using BambaIba.SharedKernel;

namespace BambaIba.Application.Abstractions.Interfaces;

public interface IKeycloakAuthService
{
    Task<TokenResponseDto?> ExchangeCredentialsForTokenAsync(string email, string password);
    Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken);
    Task<Result<bool>> CreateUserInKeycloak(RegisterCommand request);
    Task<User> GetUserFromTokenAsync(string identityId, string email, string civilStatus);
    Task<Result<AuthResultDto>> RegisterAsync(RegisterCommand request);
    Task UpdateUserAsync(string keycloakUserId, string firstName, string lastName);
}
