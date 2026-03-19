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
public class StudioMediaEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/studio/media")
            .WithTags("Studio - Media")
            .RequireAuthorization();

        // Upload vidéo (multipart/form-data)
        group.MapPost("/upload", UploadMedia)           
            .DisableAntiforgery()  // Pour multipart
            .Accepts<UploadMediaCommand>("multipart/form-data")
            .WithName("UploadMedia");

        // Supprimer une vidéo
        group.MapDelete("/{id:guid}", DeleteMedia)
            .RequireAuthorization()
            .Produces(204)
            .Produces(404)
            .WithName("DeleteMedia");

        group.MapPost("/{mediaId}/retry", RetryProcessing)
            .RequireAuthorization()
            .WithName("RetryMediaProcessing");
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

        string title = request.Title;
        if (string.IsNullOrEmpty(title) || title is "string")
            title = request.MediaFile.FileName.ToString();

        var command = new UploadMediaCommand(
            title,
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
        //return Results.Ok(result.Value);
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

    private static async Task<IResult> RetryProcessing(
        Guid mediaId,
        IMessageBus bus,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        var command = new RetryProcessingCommand(mediaId);

        Result<Result> result =
            await bus.InvokeAsync<Result>(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
}
