using MediatR;

namespace QuickChat.Message.Domain.Events;

public class MessageAddedDomainEvent : INotification
{
    public required Entities.Message Message { get; init; }
}
