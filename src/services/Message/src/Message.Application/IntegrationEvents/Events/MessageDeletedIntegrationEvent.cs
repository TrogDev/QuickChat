using QuickChat.EventBus.Events;

namespace QuickChat.Message.Application.IntegrationEvents.Events;

public record MessageDeletedIntegrationEvent : IntegrationEvent
{
    public required Domain.Entities.Message Message { get; init; }
}
