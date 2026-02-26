using System.Text.Json;
using BambaIba.ApI.Hubs;
using BambaIba.Application.Features.LiveChats;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Wolverine;

namespace BambaIba.Api.Hubs;

public class AzuraCastPollingService : BackgroundService, IAzuraCastPollingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHubContext<LiveChatHub> _hubContext;
    private readonly IConnectionMultiplexer _redis;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RadioLiveOptions _options;

    private string? _currentStationId;

    public AzuraCastPollingService(
        IHttpClientFactory httpClientFactory,
        IHubContext<LiveChatHub> hubContext,
        IConnectionMultiplexer redis,
        IServiceScopeFactory scopeFactory,
        IOptions<RadioLiveOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _hubContext = hubContext;
        _redis = redis;
        _scopeFactory = scopeFactory;
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
                    // 1. Récupération des infos AzuraCast
                    string url = $"{_options.StreamUrl}/nowplaying/{_currentStationId}";
                    JsonElement response = await client.GetFromJsonAsync<JsonElement>(url, stoppingToken);

                    // Mapping DTO
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

                    // 2. Mise à jour du SignalR (Envoi des infos musicales)
                    await _hubContext.Clients.Group(_currentStationId)
                        .SendAsync("NowPlayingUpdate", dto, cancellationToken: stoppingToken);

                    // 3. GESTION DE L'ETAT DU CHAT (Redis + Archivage)
                    await ManageLiveStatusAsync(dto/*, stoppingToken*/);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur AzuraCast station {_currentStationId}: {ex.Message}");
                }
            }

            await Task.Delay(_options.ReadIntervalMilliseconds, stoppingToken);
        }
    }

    private async Task ManageLiveStatusAsync(NowPlayingDto dto /*, CancellationToken token*/)
    {
        IDatabase db = _redis.GetDatabase();

        // Clé unique pour cette station dans Redis
        string statusKey = $"live_status:{dto.StationId}";

        // On vérifie si la clé existait juste avant (pour détecter les transitions)
        bool wasPreviouslyLive = await db.KeyExistsAsync(statusKey);

        if (dto.IsLive)
        {
            // --- CAS 1 : LE LIVE EST ACTIF ---

            // On rafraîchit la clé Redis avec une expiration (ex: 30 sec).
            // Si le service plante, le chat se ferme automatiquement par sécurité.
            await db.StringSetAsync(statusKey, "active", TimeSpan.FromSeconds(30));

            // Optionnel : Si le live vient de démarrer (transition false -> true)
            if (!wasPreviouslyLive)
            {
                Console.WriteLine($"[Radio] Live DÉMARRÉ sur station {dto.StationId} par {dto.StreamerName}");

                // Tu pourrais ici créer une entrée 'LiveSession' en DB si tu veux un historique précis
            }
        }
        else
        {
            // --- CAS 2 : LE LIVE EST INACTIF ---

            // Si la clé existait encore, ça veut dire qu'on vient de s'arrêter (transition true -> false)
            if (wasPreviouslyLive)
            {
                Console.WriteLine($"[Radio] Live TERMINÉ sur station {dto.StationId}. Archivage du chat...");

                // 1. Supprimer la clé pour fermer le chat (le Hub rejettera les nouveaux messages)
                await db.KeyDeleteAsync(statusKey);

                // 2. Lancer l'archivage du chat vers MongoDB via Wolverine
                // On utilise un scope car IMessageBus est souvent Scoped
                using IServiceScope scope = _scopeFactory.CreateScope();
                IMessageBus bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

                // Note: J'adapte la commande pour utiliser le StationId (string) ou un Guid
                // Si ton ArchiveLiveChatCommand attend un Guid, il faudra parser ou adapter la commande
                await bus.PublishAsync(new ArchiveLiveChatCommand(Guid.Parse(dto.StationId)));
            }
        }
    }
}
