// BambaIba.Api/Endpoints/PlaylistEndpoints.cs
using System.Security.Claims;
using BambaIba.Api.Extensions;
using BambaIba.Api.Infrastructure;
using BambaIba.Application.Features.Playlists.AddMediaToPlaylist;
using BambaIba.Application.Features.Playlists.CreatePlaylist;
using BambaIba.Application.Features.Playlists.GetPlaylistById;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Playlists;
using Carter;
using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;


namespace BambaIba.Api.Endpoints;

public class PlaylistEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/playlists")
            .WithTags("Playlists");

        group.MapPost("/", CreatePlaylist)
            .RequireAuthorization()
            .Produces<CreatePlaylistResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("CreatePlaylist");

        group.MapGet("/{id:guid}", GetPlaylist)
            .Produces<PlaylistDetailResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("GetPlaylist");

        group.MapPost("/{id:guid}/videos", AddVideo)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("AddVideoToPlaylist");

        // Autres routes...
    }

    private static async Task<IResult> CreatePlaylist(
        CreatePlaylistRequest request,
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        var command = new CreatePlaylistCommand
        {
            //UserId = userId,
            Name = request.Name,
            Description = request.Description,
            IsPublic = request.IsPublic
        };

        Result<CreatePlaylistResult> result = await mediator.SendCommandAsync<CreatePlaylistCommand, Result<CreatePlaylistResult>>(
            command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> GetPlaylist(
        Guid id,
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        var query = new GetPlaylistByIdQuery
        {
            PlaylistId = id,
            //CurrentUserId = userId
        };

        Result<PlaylistDetailResult?>? result = await mediator
            .SendQueryAsync<GetPlaylistByIdQuery, Result<PlaylistDetailResult?>>(
            query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> AddVideo(
        Guid id,
        AddMediaToPlaylistCommand command,
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        //var command = new AddVideoToPlaylistCommand
        //{
        //    PlaylistId = id,
        //    VideoId = request.VideoId,
        //    //UserId = userId!
        //};

        Result<AddMediaToPlaylistResult> result = await mediator
            .SendCommandAsync<AddMediaToPlaylistCommand, Result<AddMediaToPlaylistResult>>(
            command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
}
