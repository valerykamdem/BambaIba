using System.Security.Claims;
using BambaIba.Api.Extensions;
using BambaIba.Api.Infrastructure;
using BambaIba.Application.Extensions;
using BambaIba.Application.Features.MediaBase.DeleteMedia;
using BambaIba.Application.Features.MediaBase.GetMediaById;
using BambaIba.Application.Features.MediaBase.GetMedia;
using BambaIba.Application.Features.MediaBase.UploadMedia;
using BambaIba.SharedKernel;
using Carter;
using Cortex.Mediator;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;


namespace BambaIba.Api.Endpoints;

// BambaIba.Api/Endpoints/MediaEndpoints.cs
public class MediaEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/media")
            .WithTags("media")
            .WithOpenApi();

        // Upload vidéo (multipart/form-data)
        group.MapPost("/upload", UploadMedia)
            .RequireAuthorization()
            .DisableAntiforgery()  // Pour multipart
            .Accepts<UploadMediaRequest>("multipart/form-data")
            .Produces<UploadMediaResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithValidation<UploadMediaRequest>()
            .WithName("UploadMedia");

        // Liste des Media (query parameters)
        group.MapGet("/", GetMedia)
            .Produces<GetMediaResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("Getmedia")
            .WithDescription("Get paginated list of media");

        // Détail d'une vidéo (route parameter)
        group.MapGet("/{id:guid}", GetMediaById)
            .Produces<GetMediaResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("GetMediaById")
            .WithTags("media");

        // Supprimer une vidéo
        group.MapDelete("/{id:guid}", DeleteMedia)
            .RequireAuthorization()
            .Produces(204)
            .Produces(404)
            .WithName("DeleteMedia")
            .WithTags("media");

        //// Editer une vidéo
        //group.MapPut("/{id:guid}", UpdateMedia)
        //    .RequireAuthorization()
        //    .Produces(204)
        //    .Produces(404)
        //    .WithName("UpdateMedia")
        //    .WithTags("media");

    }

    // Handler pour Upload (avec Request object)
    private static async Task<IResult> UploadMedia(
        //HttpRequest httpRequest,
        [FromForm] UploadMediaRequest request,  // ← Request binding
        IMediator mediator, ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {

        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        if (request.MediaFile == null || request.MediaFile.Length == 0)
            return Results.BadRequest("Media file is required");

        string type = DetectMediaType(request.MediaFile);

        // Mapper Request → Command
        var command = new UploadMediaCommand
        {
            Title = request.Title,
            Description = request.Description,
            UserId = userId,
            MediaFile = request.MediaFile.OpenReadStream(),
            FileName = request.MediaFile.FileName,
            FileSize = request.MediaFile.Length,
            ContentType = request.MediaFile.ContentType,
            Tags = request.Tags.Any() ? request.Tags : null,
            ThumbnailFileName = request.ThumbnailFile?.FileName,
            ThumbnailStream = request.ThumbnailFile?.OpenReadStream(),
            Speaker = request.Speaker,
            Category = request.Category,
            Topic = request.Topic,
            Type = type
        };

        Result<UploadMediaResult> result = await mediator
            .SendCommandAsync<UploadMediaCommand, Result<UploadMediaResult>>(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    // Handler pour Getmedia (avec Request object pour query params)
    private static async Task<IResult> GetMedia(
        [AsParameters] GetMediaRequest request,  // ← Query binding
        IMediator mediator, CancellationToken cancellationToken)
    {
        var query = new GetMediaQuery
        (
            request.Page,
            request.PageSize,
            request.Search
        );

        Result<PagedResult<MediaDto>> result = await mediator
            .SendQueryAsync<GetMediaQuery, Result<PagedResult<MediaDto>>>(query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    // Handler pour GetMediaById
    private static async Task<IResult> GetMediaById(
        Guid id,  // ← Route parameter (simple, pas besoin de Request)
        IMediator mediator, CancellationToken cancellationToken)
    {
        var query = new GetMediaByIdQuery { MediaId = id };

        Result<MediaWithQualitiesResult>? result = await mediator
            .SendQueryAsync<GetMediaByIdQuery, Result<MediaWithQualitiesResult>>(query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    // Handler pour Delete
    private static async Task<IResult> DeleteMedia(
        Guid id, IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string identityId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(identityId))
            return Results.Unauthorized();

        var command = new DeleteMediaCommand(id);

        Result<DeleteMediaResult> result = await mediator
            .SendCommandAsync<DeleteMediaCommand, Result<DeleteMediaResult>>(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    //private static async Task<IResult> UpdateMedia(
    //    Guid id, IMediator mediator,
    //    ClaimsPrincipal user,
    //    CancellationToken cancellationToken)
    //{
    //    return user;
    //}


    private static string DetectMediaType(IFormFile file)
    {
        string contentType = file.ContentType.ToLowerInvariant();
        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        // Détection audio
        string[] audioMimeTypes = { "audio/mpeg", "audio/mp3", "audio/wav", "audio/ogg", "audio/aac", "audio/flac" };
        string[] audioExtensions = { ".mp3", ".wav", ".ogg", ".aac", ".flac" };

        if (audioMimeTypes.Contains(contentType) || audioExtensions.Contains(extension))
            return "audio";

        // Détection vidéo
        string[] videoMimeTypes = { "video/mp4", "video/mpeg", "video/quicktime", "video/webm" };
        string[] videoExtensions = { ".mp4", ".mov", ".mpeg", ".webm" };

        if (videoMimeTypes.Contains(contentType) || videoExtensions.Contains(extension))
            return "video";

        throw new InvalidOperationException($"Unsupported media type: {contentType} / {extension}");
    }

}
