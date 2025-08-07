using MediatR;

namespace QuickChat.Message.Domain.Events;

public class MessageEditedDomainEvent : INotification
{
    public required Entities.Message Message { get; init; }
}
