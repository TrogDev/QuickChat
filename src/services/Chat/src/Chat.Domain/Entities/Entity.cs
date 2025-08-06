using MediatR;

namespace QuickChat.Chat.Domain.Entities;

public abstract class Entity<TKey> : Entity
{
    public virtual TKey Id { get; set; }
}

public abstract class Entity
{
    private readonly List<INotification> domainEvents = [];
    public IReadOnlyCollection<INotification> DomainEvents => domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        domainEvents.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }
}
