using QuickChat.EventBus.Events;
using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.IntegrationEvents.Events;

public record SystemMessageAddedIntegrationEvent : IntegrationEvent
{
    public required SystemMessageModel Message { get; init; }
}
