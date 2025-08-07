using QuickChat.EventBus.Events;
using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.Application.IntegrationEvents.Events;

public record UserJoinedChatIntegrationEvent : IntegrationEvent
{
    public required Guid ChatId { get; set; }
    public required Guid UserId { get; set; }
    public required string UserName { get; set; }
}
