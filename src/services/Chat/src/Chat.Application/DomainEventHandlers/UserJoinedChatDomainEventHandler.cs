using MediatR;
using QuickChat.Chat.Application.IntegrationEvents.Events;
using QuickChat.Chat.Domain.Entities;
using QuickChat.Chat.Domain.Events;
using QuickChat.EventBus.Abstractions;

namespace QuickChat.Chat.Application.DomainEventHandlers;

public class UserJoinedChatDomainEventHandler(IEventBus eventBus)
    : INotificationHandler<UserJoinedChatDomainEvent>
{
    private readonly IEventBus eventBus = eventBus;

    public Task Handle(UserJoinedChatDomainEvent notification, CancellationToken cancellationToken)
    {
        UserJoinedChatIntegrationEvent integrationEvent =
            new() { ChatId = notification.Chat.Id, ChatParticipant = notification.ChatParticipant };
        eventBus.PublishAsync(integrationEvent);
        return Task.CompletedTask;
    }
}
