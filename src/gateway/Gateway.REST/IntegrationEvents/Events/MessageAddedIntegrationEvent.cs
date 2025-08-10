using QuickChat.EventBus.Events;
using QuickChat.Gateway.REST.Models;

namespace QuickChat.Gateway.REST.IntegrationEvents.Events;

public record MessageAddedIntegrationEvent : IntegrationEvent
{
    public required MessageModel Message { get; init; }
}
