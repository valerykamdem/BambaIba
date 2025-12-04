

using System.Security.Claims;
using System.Text.Json;
using BambaIba.Api.Extensions;
using BambaIba.Api.Hubs;
using BambaIba.Api.Infrastructure;
using BambaIba.Application.Features.LiveStreams.EndLiveStream;
using BambaIba.Application.Features.LiveStreams.GetLiveStreams;
using BambaIba.Application.Features.LiveStreams.StartLiveStream;
using BambaIba.Domain.Enums;
using BambaIba.Domain.LiveStream;
using BambaIba.Infrastructure.Persistence;
using BambaIba.SharedKernel;
using Carter;
using Cortex.Mediator;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Wolverine.Transports;

namespace BambaIba.Api.Endpoints;

public class LiveStreamEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/live")
            .WithTags("LiveStreams")
            .WithOpenApi();

        // Streamer endpoints
        group.MapGet("/LiveStream", LiveStream)
            //.RequireAuthorization()
            .Produces<StartLiveStreamResult>(StatusCodes.Status200OK)
            .WithName("LiveStream");

        //// Streamer endpoints
        //group.MapGet("/station/{id}/listeners", Listeners)
        //    //.RequireAuthorization()
        //    .Produces<StartLiveStreamResult>(StatusCodes.Status200OK)
        //    .WithName("listeners");


        // Streamer endpoints
        group.MapPost("/start", StartStream)
            .RequireAuthorization()
            .Produces<StartLiveStreamResult>(StatusCodes.Status200OK)
            .WithName("StartLiveStream");

        group.MapPost("/end/{streamId:guid}", EndStream)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .WithName("EndLiveStream");

        // Viewer endpoints
        group.MapGet("/active", GetActiveLiveStreams)
            .Produces<GetLiveStreamsResult>(StatusCodes.Status200OK)
            .WithName("GetActiveLiveStreams");

        //group.MapGet("/{streamId:guid}", GetLiveStreamDetails)
        //    .Produces<LiveStreamDetailDto>(StatusCodes.Status200OK)
        //    .WithName("GetLiveStreamDetails");

        // RTMP callback endpoints (appelés par nginx-rtmp)
        group.MapPost("/auth", AuthenticateRtmpStream)
            .WithName("AuthenticateRtmpStream");

        group.MapPost("/started", OnStreamStarted)
            .WithName("OnStreamStarted");

        group.MapPost("/ended", OnStreamEnded)
            .WithName("OnStreamEnded");
    }

    private static async Task<IResult> LiveStream(
        IHttpClientFactory httpClientFactory,
        CancellationToken cancellationToken)
    {
        HttpClient client = httpClientFactory.CreateClient();
        HttpResponseMessage response = await client.GetAsync("http://localhost:8005/api/nowplaying", cancellationToken);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();
        return Results.Content(json, "application/json");
    }

    //private static async Task<IResult> Listeners(
    //    int id,
    //   IHttpClientFactory httpClientFactory,
    //   CancellationToken cancellationToken)
    //{
    //    HttpClient client = httpClientFactory.CreateClient();
    //    HttpResponseMessage response = await client.GetAsync($"http://localhost:8005/api/nowplaying/{id}", cancellationToken);
    //    response.EnsureSuccessStatusCode();

    //    JsonElement json = await response.Content.ReadFromJsonAsync<JsonElement>();
    //    JsonElement listeners = json.GetProperty("listeners");

    //    return Results.Json(new
    //    {
    //        total = listeners.GetProperty("total").GetInt32(),
    //        unique = listeners.GetProperty("unique").GetInt32()
    //    });
    //}


    private static async Task<IResult> StartStream(
        StartLiveStreamRequest request,
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        var command = new StartLiveStreamCommand
        {
            StreamerId = userId,
            Title = request.Title,
            Description = request.Description
        };

        Result<StartLiveStreamResult> result = await mediator
            .SendCommandAsync<StartLiveStreamCommand, Result<StartLiveStreamResult>>(
            command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> EndStream(
        Guid streamId,
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        var command = new EndLiveStreamCommand
        {
            StreamId = streamId,
            StreamerId = userId!
        };

        Result<EndLiveStreamResult> result = await mediator
            .SendCommandAsync<EndLiveStreamCommand, Result<EndLiveStreamResult>>(
            command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> GetActiveLiveStreams(
        [AsParameters] GetLiveStreamsRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetLiveStreamsQuery
        {
            Page = request.Page,
            PageSize = request.PageSize,
            OnlyLive = true
        };

        Result<GetLiveStreamsResult> result = await mediator
            .SendQueryAsync<GetLiveStreamsQuery, Result<GetLiveStreamsResult>>(
            query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    //private static async Task<IResult> GetLiveStreamDetails(
    //    Guid streamId,
    //    IMediator mediator,
    //    CancellationToken cancellationToken)
    //{
    //    var query = new GetLiveStreamDetailsQuery { StreamId = streamId };

    //    Result<LiveStreamDetailDto?> result = await mediator
    //        .SendQueryAsync<GetLiveStreamDetailsQuery, Result<LiveStreamDetailDto?>>(
    //        query, cancellationToken);

    //    return result.Match(Results.Ok, CustomResults.Problem);
    //}

    // Callback RTMP : Authentifier le stream
    private static async Task<IResult> AuthenticateRtmpStream(
        HttpRequest request,
        BambaIbaDbContext context)
    {
        IFormCollection form = await request.ReadFormAsync();
        string streamKey = form["name"].ToString(); // nginx envoie le stream key

        LiveStream? stream = await context.LiveStreams
            .FirstOrDefaultAsync(s => s.StreamKey == streamKey
                                   && s.Status != LiveStreamStatus.Ended);

        // Retourner 200 = autorisé, 403 = refusé
        return stream != null ? Results.Ok() : Results.Forbid();
    }

    // Callback RTMP : Stream a démarré
    private static async Task<IResult> OnStreamStarted(
        HttpRequest request,
        BambaIbaDbContext context,
        IHubContext<LiveHub> hubContext)
    {
        IFormCollection form = await request.ReadFormAsync();
        string streamKey = form["name"].ToString();

        LiveStream? stream = await context.LiveStreams
            .FirstOrDefaultAsync(s => s.StreamKey == streamKey);

        if (stream != null)
        {
            stream.Status = LiveStreamStatus.Live;
            stream.StartedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            // Notifier via SignalR
            await hubContext.Clients.All.SendAsync("StreamStarted", new
            {
                StreamId = stream.Id,
                stream.Title,
                stream.StreamerId
            });
        }

        return Results.Ok();
    }

    // Callback RTMP : Stream terminé
    private static async Task<IResult> OnStreamEnded(
        HttpRequest request,
        BambaIbaDbContext context,
        IHubContext<LiveHub> hubContext)
    {
        IFormCollection form = await request.ReadFormAsync();
        string streamKey = form["name"].ToString();

        LiveStream? stream = await context.LiveStreams
            .FirstOrDefaultAsync(s => s.StreamKey == streamKey);

        if (stream != null)
        {
            stream.Status = LiveStreamStatus.Ended;
            stream.EndedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            // Notifier via SignalR
            await hubContext.Clients.Group(stream.Id.ToString())
                .SendAsync("StreamEnded", new
                {
                    StreamId = stream.Id,
                    Message = "Le stream est terminé"
                });
        }

        return Results.Ok();
    }
}
