using QuickChat.EventBus.Events;
using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.IntegrationEvents.Events;

public record UserJoinedChatIntegrationEvent : IntegrationEvent
{
    public required Guid ChatId { get; init; }
    public required ChatParticipantModel ChatParticipant { get; init; }
}
