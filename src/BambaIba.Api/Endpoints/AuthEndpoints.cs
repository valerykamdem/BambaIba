using BambaIba.Api.Extensions;
using BambaIba.Api.Infrastructure;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Features.Auth.Login;
using BambaIba.Application.Features.Auth.RefreshToken;
using BambaIba.Application.Features.Auth.Register;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Videos;
using Carter;
using Cortex.Mediator;
using FluentValidation;
using FluentValidation.Results;
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

        //return result.Match(Results.Ok, CustomResults.Problem);
        return Results.Ok(result);
    }

    private static async Task<IResult> RegisterEndpoint(
        [FromBody] RegisterCommand command, IMediator mediator,
         CancellationToken cancellationToken)
    {

        Result<AuthResultDto> result = await mediator
            .SendCommandAsync<RegisterCommand, Result<AuthResultDto>>(command, cancellationToken);

        return Results.Ok(result);
    }

    private static async Task<IResult> RefreshTokenEndpoint(
        [FromBody] RefreshTokenCommand command, IMediator mediator,
        IValidator<RefreshTokenCommand> validator, CancellationToken cancellationToken)
    {
        //ValidationResult validationResult = await validator.ValidateAsync(command, cancellationToken);

        //if (!validationResult.IsValid)
        //    return Results.ValidationProblem(validationResult.ToDictionary());

        // Appel à Keycloak pour obtenir le token
        Result<TokenResponseDto> result = await mediator
            .SendCommandAsync<RefreshTokenCommand, Result<TokenResponseDto>>(command, cancellationToken);
        
        //return result.Match(Results.Ok, CustomResults.Problem);
        return Results.Ok(result);
    }
}
