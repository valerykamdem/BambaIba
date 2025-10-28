using System.Security.Claims;
using BambaIba.Application.Common.Dtos;
using BambaIba.Application.Features.Videos.Upload;
using BambaIba.SharedKernel.Videos;
using Carter;
using Cortex.Mediator;
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
            .Produces<UploadVideoResponse>(200)
            .Produces<ProblemDetails>(400)
            .WithName("UploadVideo")
            .WithTags("Videos");

        //// Liste des vidéos (query parameters)
        //app.MapGet("/", GetVideos)
        //    .Produces<List<VideoDto>>(200)
        //    .WithName("GetVideos")
        //    .WithTags("Videos");

        //// Détail d'une vidéo (route parameter)
        //app.MapGet("/{id:guid}", GetVideoById)
        //    .Produces<VideoDetailDto>(200)
        //    .Produces(404)
        //    .WithName("GetVideoById")
        //    .WithTags("Videos");

        //// Supprimer une vidéo
        //app.MapDelete("/{id:guid}", DeleteVideo)
        //    .RequireAuthorization()
        //    .Produces(204)
        //    .Produces(404)
        //    .WithName("DeleteVideo")
        //    .WithTags("Videos");

        //return group;
    }

    // Handler pour Upload (avec Request object)
    private static async Task<IResult> UploadVideo(
        [FromForm] UploadVideoRequest request,  // ← Request binding
        IMediator mediator,
        ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        //var userId = user.GetUserId();
        //if (string.IsNullOrEmpty(userId))
        //    return Results.Unauthorized();

        // Mapper Request → Command
        var command = new UploadVideoCommand
        {
            Title = request.Title,
            Description = request.Description,
            UserId = "325AE7A1-1346-4146-902C-537B84A4A2EE",
            //FileStream = request.File.OpenReadStream(),
            File = request.File,
            FileName = request.File.FileName,
            FileSize = request.File.Length,
            ContentType = request.File.ContentType,
            Tags = request.Tags ?? []
        };

        UploadVideoResult result = await mediator.SendCommandAsync<UploadVideoCommand, UploadVideoResult>(command, cancellationToken);

        if (!result.IsSuccess)
            return Results.BadRequest(new ProblemDetails
            {
                Detail = result.ErrorMessage
            });

        // Mapper Result → Response
        var response = new UploadVideoResponse
        {
            VideoId = result.VideoId.ToString(),
            Status = result.Status.ToString(),
            Message = "Video uploaded successfully",
            UploadedAt = DateTime.UtcNow
        };

        return Results.Ok(response);
    }

    //// Handler pour GetVideos (avec Request object pour query params)
    //private static async Task<IResult> GetVideos(
    //    [AsParameters] GetVideosRequest request,  // ← Query binding
    //    IMediator mediator, CancellationToken cancellationToken)
    //{
    //    var query = new GetVideosQuery
    //    {
    //        Page = request.Page,
    //        PageSize = request.PageSize,
    //        Search = request.Search
    //    };

    //    var videos = await mediator.SendQueryAsync<>(query, cancellationToken);
    //    return Results.Ok(videos);
    //}

    //// Handler pour GetVideoById
    //private static async Task<IResult> GetVideoById(
    //    Guid id,  // ← Route parameter (simple, pas besoin de Request)
    //    IMediator mediator, CancellationToken cancellationToken)
    //{
    //    var query = new GetVideoByIdQuery { VideoId = id };
    //    var video = await mediator.SendQueryAsync<GetVideoByIdQuery, GetVideosResponse>(query, cancellationToken);

    //    return video is not null
    //        ? Results.Ok(video)
    //        : Results.NotFound();
    //}

    //// Handler pour Delete
    //private static async Task<IResult> DeleteVideo(
    //    Guid id,
    //    IMediator mediator,
    //    ClaimsPrincipal user, CancellationToken cancellationToken)
    //{
    //    var userId = user.GetUserId();
    //    var command = new DeleteVideoCommand { VideoId = id, UserId = userId };

    //    var result = await mediator.SendCommandAsync(command, cancellationToken);

    //    return result.IsSuccess
    //        ? Results.NoContent()
    //        : Results.NotFound();
    //}

}
