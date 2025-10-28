namespace BambaIba.Domain.Shared;

public abstract class Entity<TEntityId>
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected Entity(TEntityId id)
    {
        Id = id;
    }

    protected Entity()
    {
        // Auto-génération du Guid v7 pour tous les nouveaux entités
        if (typeof(TEntityId) == typeof(Guid))
        {
            // Utilisation de reflection pour assigner la valeur
            System.Reflection.PropertyInfo? idProperty = GetType().GetProperty("Id");
            if (idProperty != null && idProperty.CanWrite)
            {
                idProperty.SetValue(this, Guid.CreateVersion7());
            }
        }
    }

    public TEntityId Id { get; init; }

    public string? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOnUtc { get; set; }

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => [.. _domainEvents];

    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void RemoveDomainEvent(IDomainEvent domainEvent) => _domainEvents.Remove(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();


}
