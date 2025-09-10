using QuickChat.EventBus.Events;
using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.IntegrationEvents.Events;

public record MessageAddedIntegrationEvent : IntegrationEvent
{
    public required MessageModel Message { get; init; }
}
