using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string CivilStatus,
    string FirstName,
    string LastName) : ICommand<Result<AuthResultDto>>;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, Result<AuthResultDto>>
{
    private readonly IKeycloakAuthService _keycloakAuth;
    public RegisterCommandHandler(IKeycloakAuthService keycloakAuth)
    {
        _keycloakAuth = keycloakAuth ?? throw new ArgumentNullException(nameof(keycloakAuth));
    }
    public async Task<Result<AuthResultDto>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        // Appel à Keycloak pour créer l'utilisateur
        Result<AuthResultDto> response = await _keycloakAuth.RegisterAsync(command);
        return response;
    }
}
