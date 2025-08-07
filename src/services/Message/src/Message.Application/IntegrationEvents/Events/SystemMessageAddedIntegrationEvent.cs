using QuickChat.EventBus.Events;
using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.Application.IntegrationEvents.Events;

public record SystemMessageAddedIntegrationEvent : IntegrationEvent
{
    public required SystemMessage Message { get; init; }
}
