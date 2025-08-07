using QuickChat.EventBus.Events;

namespace QuickChat.Message.Application.IntegrationEvents.Events;

public record MessageAddedIntegrationEvent : IntegrationEvent
{
    public required Domain.Entities.Message Message { get; init; }
}
