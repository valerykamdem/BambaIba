namespace BambaIba.Domain.Shared;
public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedOnUtc { get; set; }
}
