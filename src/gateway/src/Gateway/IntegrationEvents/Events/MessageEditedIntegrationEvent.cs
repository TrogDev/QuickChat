using QuickChat.EventBus.Events;
using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.IntegrationEvents.Events;

public record MessageEditedIntegrationEvent : IntegrationEvent
{
    public required MessageModel Message { get; init; }
}
