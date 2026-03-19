using System.Security.Claims;
using BambaIba.Api.Extensions;
using BambaIba.Api.Infrastructure;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Features.IncrementCounts;
using BambaIba.Application.Features.MediaBase.AddReactionToMedia;
using BambaIba.Application.Features.MediaBase.DeleteMedia;
using BambaIba.Application.Features.MediaBase.GetMedia;
using BambaIba.Application.Features.MediaBase.GetMediaById;
using BambaIba.Application.Features.MediaBase.GetMediaProgress;
using BambaIba.Application.Features.MediaBase.RetryProcessing;
using BambaIba.Application.Features.MediaBase.UpdateMediaProgresses;
using BambaIba.Application.Features.MediaBase.UploadMedia;
using BambaIba.SharedKernel;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine;


namespace BambaIba.Api.Endpoints;

// BambaIba.Api/Endpoints/MediaEndpoints.cs
public class StreamingMediaEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/media")
            .WithTags("Streaming - Media");

        // Liste des Media (query parameters)
        group.MapGet("/", GetMedia)
            .Produces<CursorPagedResult<MediaDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .AllowAnonymous()
            .WithName("Getmedia")
            .WithDescription("Get paginated list of media");

        // Détail d'une vidéo (route parameter)
        group.MapGet("/{id:guid}", GetMediaById)
            .Produces<CursorPagedResult<MediaDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .AllowAnonymous()
            .WithName("GetMediaById");


        group.MapPost("/{mediaId}/reaction", AddReaction)
            .RequireAuthorization()
            .Produces<CursorPagedResult<MediaDto>>(StatusCodes.Status200OK)
            .WithName("AddReactionToMedia");

        // Media Progress
        group.MapPost("/{mediaId}/progress", AddProgress)
            .RequireAuthorization()
            .WithName("AddProgress");

        group.MapGet("/{mediaId}/progress", GetProgress)
            .RequireAuthorization()
            .WithName("GetProgress");
    }

    // Handler pour Getmedia (avec Request object pour query params)
    private static async Task<IResult> GetMedia(
        [AsParameters] GetMediaQuery query,  // ← Query binding
        IMessageBus bus,
        CancellationToken cancellationToken)
    {

        Result<CursorPagedResult<MediaDto>> result =
            await bus.InvokeAsync<Result<CursorPagedResult<MediaDto>>>(query, cancellationToken);

        //return result.Match(Results.Ok, CustomResults.Problem);
        return result.Match(value => Results.Ok(value),
            error => Results.BadRequest(error));
    }

    // Handler pour GetMediaById
    private static async Task<IResult> GetMediaById(
        Guid id,  // ← Route parameter (simple, pas besoin de Request)
        IMessageBus bus, CancellationToken cancellationToken)
    {
        var query = new GetMediaByIdQuery(id);

        Result<MediaDetailsDto>? result =
            await bus.InvokeAsync<Result<MediaDetailsDto>>(query, cancellationToken);

        if (!result.IsSuccess || result.Value == null)
            //return Results.NotFound();
            return result.Match(Results.Ok, CustomResults.Problem);

        //await bus.InvokeAsync(new IncrementPlayCountCommand(id), cancellationToken);
        await bus.PublishAsync(new IncrementPlayCountCommand(id));

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> AddReaction(
        Guid mediaId,
        [FromBody] AddReactionToMediaCommand command,
        IMessageBus bus,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        AddReactionToMediaCommand cmdWithId = command with { MediaId = mediaId };

        Result<Result> result =
            await bus.InvokeAsync<Result>(cmdWithId, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> AddProgress(
    Guid mediaId,
    UpdateMediaProgressCommand command,
    IMessageBus bus, // Injection de IMessageBus
    CancellationToken CancellationToken)
    {
        // On utilise PUBLISH_ASYNC et non INVOKE_ASYNC
        // Cela envoie le message dans l'Outbox et renvoie 202 Accepted IMMÉDIATEMENT
        // L'utilisateur n'attend pas que la DB soit écrite !
        await bus.PublishAsync(command with { MediaId = mediaId.ToString() });

        // On retourne instantanément
        return Results.Accepted();
    }

    private static async Task<IResult> GetProgress(
        Guid mediaId,
        IMessageBus bus,
        CancellationToken CancellationToken)
    {
        Result<MediaProgressDto> result = 
            await bus.InvokeAsync<Result<MediaProgressDto>>(
                new GetMediaProgressQuery(mediaId), CancellationToken);

        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.NotFound();
    }
}
