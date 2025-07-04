using System.ComponentModel.DataAnnotations.Schema;

namespace SharedKernel;

public abstract class Entity : IEntity
{

    private readonly List<IDomainEvent> _domainEvents = [];
    [NotMapped]

    public List<IDomainEvent> DomainEvents => [.. _domainEvents];

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
