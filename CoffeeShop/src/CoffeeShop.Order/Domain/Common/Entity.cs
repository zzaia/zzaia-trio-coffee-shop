namespace Zzaia.CoffeeShop.Order.Domain.Common;

/// <summary>
/// Base class for domain entities.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> domainEvents = [];

    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Gets the domain events associated with this entity.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the entity.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from the entity.
    /// </summary>
    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }
}
