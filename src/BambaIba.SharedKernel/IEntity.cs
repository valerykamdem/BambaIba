namespace BambaIba.SharedKernel;
public interface IEntity
{
    IReadOnlyList<IDomainEvent> GetDomainEvents();

    void ClearDomainEvents();
}
