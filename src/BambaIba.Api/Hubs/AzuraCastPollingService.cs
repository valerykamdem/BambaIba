using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace BambaIba.Api.Hubs;

public class AzuraCastPollingService : BackgroundService, IAzuraCastPollingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHubContext<LiveHub> _hubContext;
    private readonly RadioLiveOptions _options;

    private string? _currentStationId;

    public AzuraCastPollingService(
        IHttpClientFactory httpClientFactory,
        IHubContext<LiveHub> hubContext,
        IOptions<RadioLiveOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _hubContext = hubContext;
        _options = options.Value;
    }

    public void SetStation(string stationId)
    {
        _currentStationId = stationId;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        HttpClient client = _httpClientFactory.CreateClient();

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!string.IsNullOrEmpty(_currentStationId))
            {
                try
                {
                    string url = $"{_options.StreamUrl}/nowplaying/{_currentStationId}";
                    JsonElement response = await client.GetFromJsonAsync<JsonElement>(url, stoppingToken);


                    var dto = new NowPlayingDto
                    {
                        StationId = _currentStationId!,
                        StationName = response.GetProperty("station").GetProperty("name").GetString() ?? "",

                        Title = response.GetProperty("now_playing").GetProperty("song").GetProperty("title").GetString() ?? "",
                        Artist = response.GetProperty("now_playing").GetProperty("song").GetProperty("artist").GetString() ?? "",
                        Duration = response.GetProperty("now_playing").GetProperty("duration").GetInt32(),
                        Elapsed = response.GetProperty("now_playing").GetProperty("elapsed").GetInt32(),

                        CurrentListeners = response.GetProperty("listeners").GetProperty("current").GetInt32(),
                        UniqueListeners = response.GetProperty("listeners").GetProperty("unique").GetInt32(),

                        IsLive = response.TryGetProperty("live", out JsonElement liveProp) && liveProp.GetProperty("is_live").GetBoolean(),
                        StreamerName = response.TryGetProperty("live", out JsonElement liveProp2) ? liveProp2.GetProperty("streamer_name").GetString() : null,
                        BroadcastStart = response.TryGetProperty("live", out JsonElement liveProp3) && liveProp3.TryGetProperty("broadcast_start", out JsonElement startProp)
        ? DateTime.Parse(startProp.GetString()!)
        : null
                    };


                    await _hubContext.Clients.Group(_currentStationId)
                        .SendAsync("NowPlayingUpdate", dto, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur AzuraCast station {_currentStationId}: {ex.Message}");
                }
            }

            await Task.Delay(_options.ReadIntervalMilliseconds, stoppingToken);
        }
    }
}


//public class AzuraCastPollingService : BackgroundService
//{
//    private readonly IHttpClientFactory _httpClientFactory;
//    private readonly IHubContext<LiveHub> _hubContext;
//    private readonly RadioLiveOptions _options;

//    // station courante choisie par l’utilisateur
//    private string? _currentStationId;

//    public AzuraCastPollingService(IHttpClientFactory httpClientFactory,
//        IHubContext<LiveHub> hubContext,
//        IOptions<RadioLiveOptions> options)
//    {
//        _httpClientFactory = httpClientFactory;
//        _hubContext = hubContext;
//        _options = options.Value;
//    }

//    // Méthode appelée par ton API quand l’utilisateur clique une station
//    public void SetStation(string stationId)
//    {
//        _currentStationId = stationId;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        HttpClient client = _httpClientFactory.CreateClient();

//        while (!stoppingToken.IsCancellationRequested)
//        {
//            if (!string.IsNullOrEmpty(_currentStationId))
//            {
//                try
//                {
//                    string url = $"{_options.StreamUrl}/nowplaying/{_currentStationId}";
//                    JsonElement response = await client.GetFromJsonAsync<JsonElement>(url, stoppingToken);

//                    await _hubContext.Clients.Group(_currentStationId)
//                        .SendAsync("NowPlayingUpdate", response, cancellationToken: stoppingToken);
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Erreur AzuraCast station {_currentStationId}: {ex.Message}");
//                }
//            }

//            await Task.Delay(_options.ReadIntervalMilliseconds, stoppingToken);
//        }
//    }
//}
