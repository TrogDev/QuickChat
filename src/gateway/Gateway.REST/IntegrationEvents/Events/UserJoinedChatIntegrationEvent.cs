using QuickChat.EventBus.Events;
using QuickChat.Gateway.REST.Models;

namespace QuickChat.Gateway.REST.IntegrationEvents.Events;

public record UserJoinedChatIntegrationEvent : IntegrationEvent
{
    public required Guid ChatId { get; init; }
    public required ChatParticipantModel ChatParticipant { get; init; }
}
