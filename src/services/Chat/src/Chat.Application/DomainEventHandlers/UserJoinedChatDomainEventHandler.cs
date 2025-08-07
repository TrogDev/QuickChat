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
            new()
            {
                ChatId = notification.Chat.Id,
                UserId = notification.User.UserId,
                UserName = notification.User.Name
            };
        await eventBus.PublishAsync(integrationEvent);
    }
}
