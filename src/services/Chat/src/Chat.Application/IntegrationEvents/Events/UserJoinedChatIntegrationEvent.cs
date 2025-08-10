using QuickChat.Chat.Domain.Entities;
using QuickChat.EventBus.Events;

namespace QuickChat.Chat.Application.IntegrationEvents.Events;

public record UserJoinedChatIntegrationEvent : IntegrationEvent
{
    public required Guid ChatId { get; init; }
    public required ChatParticipant ChatParticipant { get; init; }
}
