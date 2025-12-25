namespace BambaIba.Api.Hubs;

public class RadioLiveOptions
{
    public string StreamUrl { get; set; } = string.Empty;
    public int BufferSizeBytes { get; set; }
    public int ReadIntervalMilliseconds { get; set; }
}
