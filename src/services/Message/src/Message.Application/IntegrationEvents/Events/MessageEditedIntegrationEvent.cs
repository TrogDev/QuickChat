using QuickChat.EventBus.Events;

namespace QuickChat.Message.Application.IntegrationEvents.Events;

public record MessageEditedIntegrationEvent : IntegrationEvent
{
    public required Domain.Entities.Message Message { get; init; }
}
