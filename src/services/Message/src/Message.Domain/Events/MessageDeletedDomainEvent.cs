using MediatR;

namespace QuickChat.Message.Domain.Events;

public class MessageDeletedDomainEvent : INotification
{
    public required Entities.Message Message { get; init; }
}
