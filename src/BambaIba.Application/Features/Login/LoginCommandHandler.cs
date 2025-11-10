using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Users;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.Login;


public record LoginCommand(string Email, string Password) : ICommand<Result<AuthResultDto>>;

public class LoginCommandHandler : ICommandHandler<LoginCommand, Result<AuthResultDto>>
{
    private readonly IKeycloakAuthService _keycloakAuth;

    public LoginCommandHandler(IKeycloakAuthService keycloakAuth)
    {
        _keycloakAuth = keycloakAuth ?? throw new ArgumentNullException(nameof(keycloakAuth));
    }

    public async Task<Result<AuthResultDto>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        // Appel à Keycloak pour obtenir le token
        TokenResponseDto? tokenResponse = await _keycloakAuth.ExchangeCredentialsForTokenAsync(command.Email, command.Password);
        if (tokenResponse == null)
            return Result.Failure<AuthResultDto>(Error.Failure("400", "Echec de connexion"));

        // Créer ou récupérer l'utilisateur dans ton app
        User user = await _keycloakAuth.GetUserFromTokenAsync(tokenResponse.UserId, tokenResponse.Access_Token, string.Empty);

        return Result.Success<AuthResultDto>(new AuthResultDto(
            user.Id.ToString(),
            user.Email,
            [.. user.UserRoles.Select(r => r.Role.Name)],
            tokenResponse.Access_Token,
            tokenResponse.Refresh_Token
        ));
    }
}
