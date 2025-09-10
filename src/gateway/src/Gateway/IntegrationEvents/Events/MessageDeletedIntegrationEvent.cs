using QuickChat.EventBus.Events;
using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.IntegrationEvents.Events;

public record MessageDeletedIntegrationEvent : IntegrationEvent
{
    public required MessageModel Message { get; init; }
}
