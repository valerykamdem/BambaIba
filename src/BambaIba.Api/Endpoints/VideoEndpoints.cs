using System.Security.Claims;
using BambaIba.Api.Extensions;
using BambaIba.Api.Infrastructure;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Features.Videos.DeleteVideo;
using BambaIba.Application.Features.Videos.GetVideoById;
using BambaIba.Application.Features.Videos.GetVideos;
using BambaIba.Application.Features.Videos.Upload;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Videos;
using Carter;
using Cortex.Mediator;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;


namespace BambaIba.Api.Endpoints;

// BambaIba.Api/Endpoints/VideoEndpoints.cs
public class VideoEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/videos")
            .WithTags("Videos")
            .WithOpenApi();

        // Upload vidéo (multipart/form-data)
        group.MapPost("/upload", UploadVideo)
            //.RequireAuthorization()
            .DisableAntiforgery()  // Pour multipart
            .Accepts<UploadVideoRequest>("multipart/form-data")
            .Produces<UploadVideoResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithValidation<UploadVideoRequest>()
            .RequireAuthorization()
            .WithName("UploadVideo");

        // Liste des vidéos (query parameters)
        group.MapGet("/", GetVideos)
            .Produces<GetVideosResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("GetVideos")
            .WithDescription("Get paginated list of videos");

        // Détail d'une vidéo (route parameter)
        group.MapGet("/{id:guid}", GetVideoById)
            .Produces<GetVideosResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("GetVideoById")
            .WithTags("Videos");

        // Supprimer une vidéo
        group.MapDelete("/{id:guid}", DeleteVideo)
            .RequireAuthorization()
            .Produces(204)
            .Produces(404)
            .WithName("DeleteVideo")
            .WithTags("Videos");

        //return group;
    }

    // Handler pour Upload (avec Request object)
    private static async Task<IResult> UploadVideo(
        [FromForm] UploadVideoRequest request,  // ← Request binding
        IMediator mediator, ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {

        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        // Mapper Request → Command
        var command = new UploadVideoCommand
        {
            Title = request.Title,
            Description = request.Description,
            UserId = userId,
            //FileStream = request.File.OpenReadStream(),
            File = request.File,
            FileName = request.File.FileName,
            FileSize = request.File.Length,
            ContentType = request.File.ContentType,
            Tags = request.Tags ?? []
        };

        Result<UploadVideoResult> result = await mediator
            .SendCommandAsync<UploadVideoCommand, Result<UploadVideoResult>>(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    // Handler pour GetVideos (avec Request object pour query params)
    private static async Task<IResult> GetVideos(
        GetVideosRequest request,  // ← Query binding
        IMediator mediator, CancellationToken cancellationToken)
    {
        var query = new GetVideosQuery
        (
            request.Page > 0 ? (int)request.Page : 1,
            request.PageSize > 0 ? (int)request.PageSize : 20,
            string.IsNullOrWhiteSpace(request.Search) ? null : request.Search
        );

        Result<GetVideosResult> result = await mediator
            .SendQueryAsync<GetVideosQuery, Result<GetVideosResult>>(query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    // Handler pour GetVideoById
    private static async Task<IResult> GetVideoById(
        Guid id,  // ← Route parameter (simple, pas besoin de Request)
        IMediator mediator, CancellationToken cancellationToken)
    {
        var query = new GetVideoByIdQuery { VideoId = id };

        Result<VideoDetailResult>? result = await mediator
            .SendQueryAsync<GetVideoByIdQuery, Result<VideoDetailResult>>(query, cancellationToken);

        //return video is not null
        //    ? Results.Ok(video)
        //    : Results.NotFound();

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    // Handler pour Delete
    private static async Task<IResult> DeleteVideo(
        Guid id, IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string identityId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(identityId))
            return Results.Unauthorized();

        var command = new DeleteVideoCommand(id);

        Result<DeleteVideoResult> result = await mediator
            .SendCommandAsync<DeleteVideoCommand, Result<DeleteVideoResult>>(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

}
