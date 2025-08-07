using QuickChat.Chat.Domain.Entities;
using QuickChat.EventBus.Events;

namespace QuickChat.Chat.Application.IntegrationEvents.Events;

public record UserJoinedChatIntegrationEvent : IntegrationEvent
{
    public required Guid ChatId { get; set; }
    public required Guid UserId { get; set; }
    public required string UserName { get; set; }
}
