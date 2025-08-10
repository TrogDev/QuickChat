using QuickChat.EventBus.Events;
using QuickChat.Gateway.REST.Models;

namespace QuickChat.Gateway.REST.IntegrationEvents.Events;

public record SystemMessageAddedIntegrationEvent : IntegrationEvent
{
    public required SystemMessageModel Message { get; init; }
}
