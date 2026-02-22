using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.SharedKernel;

namespace BambaIba.Application.Features.Auth.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string CivilStatus,
    string FirstName,
    string LastName);

public class RegisterCommandHandler(IKeycloakAuthService keycloakAuth)
{
    public async Task<Result<AuthResultDto>> Handle(RegisterCommand command)
    {
        // Appel à Keycloak pour créer l'utilisateur
        Result<AuthResultDto> response = await keycloakAuth.RegisterAsync(command);
        return response;
    }
}
