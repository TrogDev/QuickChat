using QuickChat.EventBus.Events;

namespace QuickChat.Message.Application.IntegrationEvents.Events;

public record UserJoinedChatIntegrationEvent : IntegrationEvent
{
    public required Guid ChatId { get; init; }
    public required ChatParticipantModel ChatParticipant { get; init; }

    public record ChatParticipantModel
    {
        public required Guid Id { get; init; }
        public required Guid UserId { get; init; }
        public required string Name { get; init; }
    }
}
