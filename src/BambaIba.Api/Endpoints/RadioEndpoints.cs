using BambaIba.Api.Hubs;
using Carter;
using Microsoft.Extensions.Options;

namespace BambaIba.Api.Endpoints;

public class RadioEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/radio")
            .WithTags("LiveStreams");

        // Streamer endpoints
        group.MapGet("/LiveStream", LiveStream)
            //.Produces<StartLiveStreamResult>(StatusCodes.Status200OK)
            .WithName("LiveStream");

        // Streamer endpoints
        group.MapGet("/Stations", GetStations)
            //.Produces<StartLiveStreamResult>(StatusCodes.Status200OK)
            .WithName("listeners");


        // Streamer endpoints
        group.MapPost("/selectStation/{stationId}", SelectStation)
            //.Produces<StartLiveStreamResult>(StatusCodes.Status200OK)
            .WithName("SelectStation");

        //    group.MapPost("/end/{streamId:guid}", EndStream)
        //        .RequireAuthorization()
        //        .Produces(StatusCodes.Status204NoContent)
        //        .WithName("EndLiveStream");

        //    // Viewer endpoints
        //    group.MapGet("/active", GetActiveLiveStreams)
        //        .Produces<GetLiveStreamsResult>(StatusCodes.Status200OK)
        //        .WithName("GetActiveLiveStreams");

        //    //group.MapGet("/{streamId:guid}", GetLiveStreamDetails)
        //    //    .Produces<LiveStreamDetailDto>(StatusCodes.Status200OK)
        //    //    .WithName("GetLiveStreamDetails");

        //    // RTMP callback endpoints (appelés par nginx-rtmp)
        //    group.MapPost("/auth", AuthenticateRtmpStream)
        //        .WithName("AuthenticateRtmpStream");

        //    group.MapPost("/started", OnStreamStarted)
        //        .WithName("OnStreamStarted");

        //    group.MapPost("/ended", OnStreamEnded)
        //        .WithName("OnStreamEnded");
    }

    private static async Task<IResult> LiveStream(
        IHttpClientFactory httpClientFactory,
        IOptions<RadioLiveOptions> options,
        CancellationToken cancellationToken)
    {
        HttpClient client = httpClientFactory.CreateClient();
        HttpResponseMessage response = await client.GetAsync($"{options.Value.StreamUrl}/nowplaying", cancellationToken);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        return Results.Content(json, "application/json");
    }

    private static async Task<IResult> GetStations(
       IHttpClientFactory httpClientFactory,
       IOptions<RadioLiveOptions> options,
       CancellationToken cancellationToken)
    {
        HttpClient client = httpClientFactory.CreateClient();
        HttpResponseMessage response = await client.GetAsync($"{options.Value.StreamUrl}/stations", cancellationToken);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        return Results.Content(json, "application/json");
    }

    private static async Task<IResult> SelectStation(
        int stationId,
        IAzuraCastPollingService pollingService)
    {
        pollingService.SetStation(stationId.ToString());
        return Results.Ok($"Station {stationId} selected");
    }

    //private static async Task<IResult> EndStream(
    //    Guid streamId,
    //    IMediator mediator,
    //    ClaimsPrincipal user,
    //    CancellationToken cancellationToken)
    //{
    //    string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
    //              ?? user.FindFirstValue("sub");

    //    var command = new EndLiveStreamCommand
    //    {
    //        StreamId = streamId,
    //        StreamerId = userId!
    //    };

    //    Result<EndLiveStreamResult> result = await mediator
    //        .SendCommandAsync<EndLiveStreamCommand, Result<EndLiveStreamResult>>(
    //        command, cancellationToken);

    //    return result.Match(Results.Ok, CustomResults.Problem);
    //}

    //private static async Task<IResult> GetActiveLiveStreams(
    //    [AsParameters] GetLiveStreamsRequest request,
    //    IMediator mediator,
    //    CancellationToken cancellationToken)
    //{
    //    var query = new GetLiveStreamsQuery
    //    {
    //        Page = request.Page,
    //        PageSize = request.PageSize,
    //        OnlyLive = true
    //    };

    //    Result<GetLiveStreamsResult> result = await mediator
    //        .SendQueryAsync<GetLiveStreamsQuery, Result<GetLiveStreamsResult>>(
    //        query, cancellationToken);

    //    return result.Match(Results.Ok, CustomResults.Problem);
    //}

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
    //private static async Task<IResult> AuthenticateRtmpStream(
    //    HttpRequest request,
    //    BambaIbaDbContext context)
    //{
    //    IFormCollection form = await request.ReadFormAsync();
    //    string streamKey = form["name"].ToString(); // nginx envoie le stream key

    //    LiveStream? stream = await context.LiveStreams
    //        .FirstOrDefaultAsync(s => s.StreamKey == streamKey
    //                               && s.Status != LiveStreamStatus.Ended);

    //    // Retourner 200 = autorisé, 403 = refusé
    //    return stream != null ? Results.Ok() : Results.Forbid();
    //}

    //// Callback RTMP : Stream a démarré
    //private static async Task<IResult> OnStreamStarted(
    //    HttpRequest request,
    //    BambaIbaDbContext context,
    //    IHubContext<LiveHub> hubContext)
    //{
    //    IFormCollection form = await request.ReadFormAsync();
    //    string streamKey = form["name"].ToString();

    //    LiveStream? stream = await context.LiveStreams
    //        .FirstOrDefaultAsync(s => s.StreamKey == streamKey);

    //    if (stream != null)
    //    {
    //        stream.Status = LiveStreamStatus.Live;
    //        stream.StartedAt = DateTime.UtcNow;
    //        await context.SaveChangesAsync();

    //        // Notifier via SignalR
    //        await hubContext.Clients.All.SendAsync("StreamStarted", new
    //        {
    //            StreamId = stream.Id,
    //            stream.Title,
    //            stream.StreamerId
    //        });
    //    }

    //    return Results.Ok();
    //}

    //// Callback RTMP : Stream terminé
    //private static async Task<IResult> OnStreamEnded(
    //    HttpRequest request,
    //    BambaIbaDbContext context,
    //    IHubContext<LiveHub> hubContext)
    //{
    //    IFormCollection form = await request.ReadFormAsync();
    //    string streamKey = form["name"].ToString();

    //    LiveStream? stream = await context.LiveStreams
    //        .FirstOrDefaultAsync(s => s.StreamKey == streamKey);

    //    if (stream != null)
    //    {
    //        stream.Status = LiveStreamStatus.Ended;
    //        stream.EndedAt = DateTime.UtcNow;
    //        await context.SaveChangesAsync();

    //        // Notifier via SignalR
    //        await hubContext.Clients.Group(stream.Id.ToString())
    //            .SendAsync("StreamEnded", new
    //            {
    //                StreamId = stream.Id,
    //                Message = "Le stream est terminé"
    //            });
    //    }

    //    return Results.Ok();
    //}
}
