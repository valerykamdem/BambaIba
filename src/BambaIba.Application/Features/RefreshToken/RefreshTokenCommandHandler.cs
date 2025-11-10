using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;

namespace Concertation.Banking.API.Features.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<Result<TokenResponseDto>>;


public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, Result<TokenResponseDto>>
{
    private readonly IKeycloakAuthService _keycloakAuth;

    public RefreshTokenCommandHandler(IKeycloakAuthService keycloakAuth)
    {
        _keycloakAuth = keycloakAuth ?? throw new ArgumentNullException(nameof(keycloakAuth));
    }

    public async Task<Result<TokenResponseDto>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        // Appel à Keycloak pour obtenir le token
        TokenResponseDto? tokenResponse = await _keycloakAuth.RefreshTokenAsync(command.RefreshToken);
        if (tokenResponse == null)
            return Result.Failure<TokenResponseDto>(Error.Failure("400", "Echec token"));

        return Result.Success(tokenResponse);
    }
}
