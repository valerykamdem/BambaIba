using BambaIba.Api.Extensions;
using BambaIba.Api.Infrastructure;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Features.Auth.Login;
using BambaIba.Application.Features.Auth.RefreshToken;
using BambaIba.Application.Features.Auth.Register;
using BambaIba.SharedKernel;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BambaIba.Api.Endpoints;

public class AuthEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        // Enregistrement des endpoints d'authentification
        group.MapPost("/login", LoginEndpoint)
            .WithValidation<LoginCommand>();
        group.MapPost("/register", RegisterEndpoint)
            .WithValidation<RegisterCommand>();
        group.MapPost("/refresh-token", RefreshTokenEndpoint)
            .WithValidation<RefreshTokenCommand>();
    }

    private static async Task<IResult> LoginEndpoint(
        [FromBody] LoginCommand command, IMessageBus bus,
        CancellationToken cancellationToken)
    {

        Result<AuthResultDto> result = 
            await bus.InvokeAsync<Result<AuthResultDto>>(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
        //return Results.Ok(result);
    }

    private static async Task<IResult> RegisterEndpoint(
        [FromBody] RegisterCommand command, IMessageBus bus,
         CancellationToken cancellationToken)
    {

        Result<AuthResultDto> result = 
            await bus.InvokeAsync<Result<AuthResultDto>>(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
        //return Results.Ok(result);
    }

    private static async Task<IResult> RefreshTokenEndpoint(
        [FromBody] RefreshTokenCommand command, IMessageBus bus,
        IValidator<RefreshTokenCommand> validator, CancellationToken cancellationToken)
    {

        // Appel à Keycloak pour obtenir le token
        Result<AuthResultDto> result = 
            await bus.InvokeAsync<Result<AuthResultDto>>(command, cancellationToken);
        
        return result.Match(Results.Ok, CustomResults.Problem);
        //return Results.Ok(result);
    }
}
