using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Users;
using BambaIba.SharedKernel;

namespace BambaIba.Application.Features.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken);


public class RefreshTokenHandler(IKeycloakAuthService keycloakAuth)
{

    public async Task<Result<AuthResultDto>> Handle(RefreshTokenCommand command)
    {
        // Appel à Keycloak pour obtenir le token
        TokenResponseDto? tokenResponse = await keycloakAuth.RefreshTokenAsync(command.RefreshToken);
        if (tokenResponse == null)
            return Result.Failure<AuthResultDto>(Error.Failure("Not.Token", "Not Token"));

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
