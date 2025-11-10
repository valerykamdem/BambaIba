namespace BambaIba.SharedKernel;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
