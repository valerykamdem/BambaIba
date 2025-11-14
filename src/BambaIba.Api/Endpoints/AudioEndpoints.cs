using System.Security.Claims;
using BambaIba.Api.Extensions;
using BambaIba.Api.Infrastructure;
using BambaIba.Application.Features.Audios.GetAudioById;
using BambaIba.Application.Features.Audios.GetAudios;
using BambaIba.Application.Features.Audios.Upload;
using BambaIba.SharedKernel;
using Carter;
using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace BambaIba.Api.Endpoints;

public class AudioEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/audios")
            .WithTags("Audios")
            .WithOpenApi();

        group.MapPost("/upload", UploadAudio)
            .RequireAuthorization()
            .DisableAntiforgery()
            .Accepts<UploadAudioRequest>("multipart/form-data")
            .Produces<UploadAudioResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("UploadAudio");

        group.MapGet("/", GetAudios)
            .Produces<GetAudiosResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("GetAudios");

        // Détail d'une vidéo (route parameter)
        group.MapGet("/{id:guid}", GetAudioById)
            .Produces<GetAudiosResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("GetAudioById")
            .WithTags("Audios");
    }

    private static async Task<IResult> UploadAudio(
        HttpRequest httpRequest,
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        if (!httpRequest.HasFormContentType)
            return Results.BadRequest("Invalid content type");

        IFormCollection form = await httpRequest.ReadFormAsync(cancellationToken);

        string title = form["Title"].ToString();
        string description = form["Description"].ToString();
        string artist = form["Artist"].ToString();
        string album = form["Album"].ToString();
        string genre = form["Genre"].ToString();
        var tags = form["Tags"].ToString()
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .ToList();

        IFormFile? audioFile = form.Files["AudioFile"];
        IFormFile? coverImage = form.Files["CoverImage"]; // Optionnel

        if (audioFile == null || audioFile.Length == 0)
            return Results.BadRequest("Audio file is required");

        var command = new UploadAudioCommand
        {
            Title = title,
            Description = description,
            Artist = artist,
            Album = album,
            Genre = genre,
            //UserId = userId,
            AudioStream = audioFile.OpenReadStream(),
            AudioFileName = audioFile.FileName,
            AudioFileSize = audioFile.Length,
            AudioContentType = audioFile.ContentType,
            CoverImageStream = coverImage?.OpenReadStream(),
            CoverImageFileName = coverImage?.FileName,
            Tags = tags.Any() ? tags : null
        };

        Result<UploadAudioResult> result = await mediator
            .SendCommandAsync<UploadAudioCommand, Result<UploadAudioResult>>(
            command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> GetAudios(
    [AsParameters] GetAudiosRequest request,
    IMediator mediator,
    CancellationToken cancellationToken)
    {
        var query = new GetAudiosQuery
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Genre = request.Genre,
            Search = request.Search
        };

        Result<GetAudiosResult> result = await mediator
            .SendQueryAsync<GetAudiosQuery, Result<GetAudiosResult>>(
            query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    // Handler pour GetVideoById
    private static async Task<IResult> GetAudioById(
        Guid id,  // ← Route parameter (simple, pas besoin de Request)
        IMediator mediator, CancellationToken cancellationToken)
    {
        var query = new GetAudioByIdQuery(id);

        Result<AudioDetailResult>? result = await mediator
            .SendQueryAsync<GetAudioByIdQuery, Result<AudioDetailResult>>(query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
}
