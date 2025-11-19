using BambaIba.Domain.Videos;
using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities;
public sealed class TranscodeJob : Entity<Guid>, ISoftDeletable
{

    public Guid VideoId { get; set; }
    public string SourceObject { get; set; } = default!;
    public string[] TargetRenditions { get; set; } = ["240p", "480p", "720p", "1080p"];
    public string? Error { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string Status { get; set; } = "Queued";
}
