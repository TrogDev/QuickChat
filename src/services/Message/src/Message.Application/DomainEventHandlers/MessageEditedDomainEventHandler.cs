using MediatR;
using QuickChat.EventBus.Abstractions;
using QuickChat.Message.Application.IntegrationEvents.Events;
using QuickChat.Message.Domain.Events;

namespace QuickChat.Message.Application.DomainEventHandlers;

public class MessageEditedDomainEventHandler(IEventBus eventBus)
    : INotificationHandler<MessageEditedDomainEvent>
{
    private readonly IEventBus eventBus = eventBus;

    public async Task Handle(
        MessageEditedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        MessageEditedIntegrationEvent integrationEvent = new() { Message = notification.Message };
        await eventBus.PublishAsync(integrationEvent);
    }
}
