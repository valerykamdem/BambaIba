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
using BambaIba.Application.Features.MediaBase.UpdateMediaProgresses;
using BambaIba.Application.Features.MediaBase.UploadMedia;
using BambaIba.SharedKernel;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine;


namespace BambaIba.Api.Endpoints;

// BambaIba.Api/Endpoints/MediaEndpoints.cs
public class MediaEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/media")
            .WithTags("Media");

        // Upload vidéo (multipart/form-data)
        group.MapPost("/upload", UploadMedia)
            .RequireAuthorization()
            .DisableAntiforgery()  // Pour multipart
            .Accepts<UploadMediaCommand>("multipart/form-data")
            .WithName("UploadMedia");

        // Liste des Media (query parameters)
        group.MapGet("/", GetMedia)
            .Produces<CursorPagedResult<MediaDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("Getmedia")
            .WithDescription("Get paginated list of media");

        // Détail d'une vidéo (route parameter)
        group.MapGet("/{id:guid}", GetMediaById)
            .Produces<CursorPagedResult<MediaDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("GetMediaById");

        // Supprimer une vidéo
        group.MapDelete("/{id:guid}", DeleteMedia)
            .RequireAuthorization()
            .Produces(204)
            .Produces(404)
            .WithName("DeleteMedia");

        group.MapPost("/{mediaId}/reaction", AddReaction)
            .RequireAuthorization()
            .Produces<CursorPagedResult<MediaDto>>(StatusCodes.Status200OK)
            .WithName("AddReaction");

        // Media Progress
        group.MapPost("/{mediaId}/progress", AddProgress)
            .RequireAuthorization()
            .WithName("AddProgress");

        group.MapGet("/{mediaId}/progress", GetProgress)
            .RequireAuthorization()
            .WithName("GetProgress");
    }

    // Handler pour Upload (avec Request object)
    private static async Task<IResult> UploadMedia(
        //HttpRequest httpRequest,
        [FromForm] UploadMediaRequest request,
        IMessageBus bus, ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {

        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        if (request.MediaFile == null || request.MediaFile.Length == 0)
            return Results.BadRequest("Media file is required");

        var command = new UploadMediaCommand(
            request.Title,
            request.Description,

            request.Speaker,
            request.Category,
            request.Topic,
            request.Tags,

            request.MediaFile.OpenReadStream(),
            request.MediaFile.FileName,
            request.MediaFile.ContentType,

            request.ThumbnailFile?.OpenReadStream(),
            request.ThumbnailFile?.FileName,
            request.ThumbnailFile?.ContentType
        );


        Result<UploadMediaResult> result =
            await bus.InvokeAsync<Result<UploadMediaResult>>(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
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
            return Results.NotFound();

        //await bus.InvokeAsync(new IncrementPlayCountCommand(id), cancellationToken);
        await bus.PublishAsync(new IncrementPlayCountCommand(id));

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    // Handler pour Delete
    private static async Task<IResult> DeleteMedia(
        Guid id, IMessageBus bus,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string identityId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(identityId))
            return Results.Unauthorized();

        var command = new DeleteMediaCommand(id);

        Result<DeleteMediaResult> result =
            await bus.InvokeAsync<Result<DeleteMediaResult>>(command, cancellationToken);

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
            await bus.InvokeAsync<Result<MediaProgressDto>>(new GetMediaProgressQuery(mediaId), CancellationToken);

        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.NotFound();
    }
}
