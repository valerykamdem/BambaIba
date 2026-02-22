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

public class AuthEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Enregistrement des endpoints d'authentification
        app.MapPost("/api/auth/login", LoginEndpoint)
            .WithValidation<LoginCommand>();
        app.MapPost("/api/auth/register", RegisterEndpoint)
            .WithValidation<RegisterCommand>();
        app.MapPost("/api/auth/refresh-token", RefreshTokenEndpoint)
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
        Result<TokenResponseDto> result = 
            await bus.InvokeAsync<Result<TokenResponseDto>>(command, cancellationToken);
        
        return result.Match(Results.Ok, CustomResults.Problem);
        //return Results.Ok(result);
    }
}
