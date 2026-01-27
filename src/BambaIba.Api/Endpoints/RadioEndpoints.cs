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

}
