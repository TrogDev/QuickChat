using MediatR;
using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.Domain.Events;

public class SystemMessageAddedDomainEvent : INotification
{
    public required SystemMessage Message { get; init; }
}
