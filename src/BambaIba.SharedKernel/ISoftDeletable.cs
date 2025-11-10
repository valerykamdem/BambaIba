namespace BambaIba.SharedKernel;
public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedOnUtc { get; set; }
}
