using QuickChat.Chat.Domain.Entities;
using QuickChat.EventBus.Events;

namespace QuickChat.Chat.Application.IntegrationEvents.Events;

public record UserJoinedChatIntegrationEvent : IntegrationEvent
{
    public required Domain.Entities.Chat Chat { get; init; }
    public required ChatParticipant User { get; init; }
}
