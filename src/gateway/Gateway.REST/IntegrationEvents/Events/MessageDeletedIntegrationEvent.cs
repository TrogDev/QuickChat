using QuickChat.EventBus.Events;
using QuickChat.Gateway.REST.Models;

namespace QuickChat.Gateway.REST.IntegrationEvents.Events;

public record MessageDeletedIntegrationEvent : IntegrationEvent
{
    public required MessageModel Message { get; init; }
}
