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
        RouteGroupBuilder group = app.MapGroup("/api/media/{mediaId:guid}/likes")
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
        Guid mediaId,
        [FromBody] ToggleLikeRequest request,
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        var command = new ToggleLikeCommand(mediaId, request.IsLike);

        Result<ToggleLikeResult> result = await mediator
            .SendCommandAsync<ToggleLikeCommand, Result<ToggleLikeResult>>(
            command,
            cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> GetLikeStatus(
        Guid mediaId,
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub")
                  ?? string.Empty;

        var query = new GetLikeStatusQuery(mediaId);

        Result<GetLikeStatusResult> result = await mediator
            .SendQueryAsync<GetLikeStatusQuery, Result<GetLikeStatusResult>>(
            query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
}
