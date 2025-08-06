using MediatR;
using QuickChat.Chat.Application.IntegrationEvents.Events;
using QuickChat.Chat.Domain.Events;
using QuickChat.EventBus.Abstractions;

namespace QuickChat.Chat.Application.DomainEventHandlers;

public class UserJoinedChatDomainEventHandler(IEventBus eventBus)
    : INotificationHandler<UserJoinedChatDomainEvent>
{
    private readonly IEventBus eventBus = eventBus;

    public async Task Handle(
        UserJoinedChatDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        UserJoinedChatIntegrationEvent integrationEvent =
            new() { Chat = notification.Chat, User = notification.User };
        await eventBus.PublishAsync(integrationEvent);
    }
}
