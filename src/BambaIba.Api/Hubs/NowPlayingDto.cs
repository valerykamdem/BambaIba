namespace BambaIba.Api.Hubs;

public class NowPlayingDto
{
    // Station
    public string StationId { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;

    // Infos du morceau ou flux
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public int Duration { get; set; } // en secondes
    public int Elapsed { get; set; }  // en secondes

    // Auditeurs
    public int CurrentListeners { get; set; }
    public int UniqueListeners { get; set; }

    // Infos live
    public bool IsLive { get; set; }
    public string? StreamerName { get; set; }
    public DateTime? BroadcastStart { get; set; }
}

