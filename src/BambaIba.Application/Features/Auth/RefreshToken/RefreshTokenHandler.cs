using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.SharedKernel;

namespace BambaIba.Application.Features.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken);


public class RefreshTokenHandler(IKeycloakAuthService keycloakAuth)
{

    public async Task<Result<TokenResponseDto>> Handle(RefreshTokenCommand command)
    {
        // Appel à Keycloak pour obtenir le token
        TokenResponseDto? tokenResponse = await keycloakAuth.RefreshTokenAsync(command.RefreshToken);
        if (tokenResponse == null)
            return Result.Failure<TokenResponseDto>(Error.Problem("400", "Echec token"));

        return Result.Success(tokenResponse);
    }
}
