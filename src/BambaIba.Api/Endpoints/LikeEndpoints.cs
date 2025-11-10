using System.Security.Claims;
using BambaIba.Api.Extensions;
using BambaIba.Api.Infrastructure;
using BambaIba.Application.Features.Likes.GetLikeStatus;
using BambaIba.Application.Features.Likes.ToggleLike;
using BambaIba.SharedKernel;
using Carter;
using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace BambaIba.Api.Endpoints;
public class LikeEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/videos/{videoId:guid}/likes")
            .WithTags("Likes")
            .WithOpenApi();

        group.MapPost("/", ToggleLike)
            .RequireAuthorization()
            .Produces<ToggleLikeResult>(StatusCodes.Status200OK)
            .WithName("ToggleLike");

        group.MapGet("/status", GetLikeStatus)
            .Produces<GetLikeStatusResult>(StatusCodes.Status200OK)
            .WithName("GetLikeStatus");
    }

    private static async Task<IResult> ToggleLike(
        [FromBody] ToggleLikeCommand command,
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        Result<ToggleLikeResult> result = await mediator
            .SendCommandAsync<ToggleLikeCommand, Result<ToggleLikeResult>>(
            command,
            cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> GetLikeStatus(
        Guid videoId,
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub")
                  ?? string.Empty;

        var query = new GetLikeStatusQuery(videoId);

        Result<GetLikeStatusResult> result = await mediator
            .SendQueryAsync<GetLikeStatusQuery, Result<GetLikeStatusResult>>(
            query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
}
