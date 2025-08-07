using MediatR;
using QuickChat.EventBus.Abstractions;
using QuickChat.Message.Application.IntegrationEvents.Events;
using QuickChat.Message.Domain.Events;

namespace QuickChat.Message.Application.DomainEventHandlers;

public class MessageDeletedDomainEventHandler(IEventBus eventBus)
    : INotificationHandler<MessageDeletedDomainEvent>
{
    private readonly IEventBus eventBus = eventBus;

    public async Task Handle(
        MessageDeletedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        MessageDeletedIntegrationEvent integrationEvent = new() { Message = notification.Message };
        await eventBus.PublishAsync(integrationEvent);
    }
}
