// BambaIba.Api/Endpoints/PlaylistEndpoints.cs
using System.Security.Claims;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Features.Playlists.AddMediaToPlaylists;
using BambaIba.Application.Features.Playlists.CreatePlaylist;
using BambaIba.Application.Features.Playlists.GetPlaylistDetails;
using BambaIba.Application.Features.Playlists.GetUserPlaylist;
using BambaIba.Application.Features.Playlists.RemoveFromPlaylists;
using BambaIba.SharedKernel;
using Carter;
using Microsoft.AspNetCore.Mvc;
using Wolverine;


namespace BambaIba.Api.Endpoints;

public class PlaylistEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/playlists")
            .WithTags("Playlists");

        group.MapPost("/", CreatePlaylist)
            .RequireAuthorization()
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("CreatePlaylist");

        group.MapGet("/", GetPlaylist)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("GetPlaylist");

        group.MapGet("/{id:guid}", GetPlaylistDetail)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("GetPlaylistDetails");

        group.MapPost("/{id:guid}/items", AddMediaToPlaylist)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("AddMediaToPlaylist");

        group.MapDelete("/{id:guid}/items/{mediaId}", DeletePlaylist)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("DeletePlaylist");

        // Autres routes...
    }

    private static async Task<IResult> CreatePlaylist(
        CreatePlaylistCommand command,
        IMessageBus bus,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        Result<Guid> result = await bus.InvokeAsync<Result<Guid>>(command, cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new { PlaylistId = result.Value })
            : Results.BadRequest(result.Error);
    }

    private static async Task<IResult> GetPlaylist(
        IMessageBus bus,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        var query = new GetUserPlaylistsQuery();

        Result<List<PlaylistDto>> result = 
            await bus.InvokeAsync<Result<List<PlaylistDto>>>(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound();
    }

    private static async Task<IResult> GetPlaylistDetail(
        Guid id,
        IMessageBus bus,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        var query = new GetPlaylistDetailsQuery(id);

        Result<PlaylistDetailsDto> result = 
            await bus.InvokeAsync<Result<PlaylistDetailsDto>>(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound();
    }

    // 4. Add Media to Playlist
    private static async Task<IResult> AddMediaToPlaylist(
        Guid id, // PlaylistId
        AddMediaToPlaylistCommand command, // Contient le MediaId
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        AddMediaToPlaylistCommand cmdWithId = command with { PlaylistId = id };

        SharedKernel.Result result = 
            await bus.InvokeAsync<SharedKernel.Result>(cmdWithId, cancellationToken);

        return result.IsSuccess 
            ? Results.Ok() 
            : Results.BadRequest(result.Error);
    }

    // 5. Remove Media from Playlist
    private static async Task<IResult> DeletePlaylist(
         Guid id, // PlaylistId
         Guid mediaId,
         IMessageBus bus,
         CancellationToken cancellationToken)
        {
            var cmd = new RemoveFromPlaylistCommand(id, mediaId);

        SharedKernel.Result result = 
            await bus.InvokeAsync<SharedKernel.Result>(cmd, cancellationToken);

            return result.IsSuccess
            ? Results.NoContent() 
            : Results.NotFound();
        }
}
