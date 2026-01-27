using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Users;
using BambaIba.SharedKernel;

namespace BambaIba.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password);

public class LoginHandler
{
    public static async Task<Result<AuthResultDto>> Handle(LoginCommand command, IKeycloakAuthService keycloakAuth)
    {
        // Appel à Keycloak pour obtenir le token
        TokenResponseDto? tokenResponse = await keycloakAuth.ExchangeCredentialsForTokenAsync(command.Email, command.Password);
        if (tokenResponse == null)
            return Result.Failure<AuthResultDto>(Error.Failure("400", "Echec de connexion"));

        // Créer ou récupérer l'utilisateur dans ton app
        User user = await keycloakAuth.GetUserFromTokenAsync(tokenResponse.UserId, tokenResponse.Access_Token, string.Empty);

        return Result.Success(new AuthResultDto(
            user.Id.ToString(),
            user.Email,
            [.. user.UserRoles.Select(r => r.Role.Name)],
            tokenResponse.Access_Token,
            tokenResponse.Refresh_Token
        ));
    }
}
