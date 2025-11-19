// BambaIba.Domain/Entities/Audio.cs
using BambaIba.Domain.MediaBase;

namespace BambaIba.Domain.Audios;

public sealed class Audio : Media //Entity<Guid>, ISoftDeletable
{
    public string Speaker { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
}

