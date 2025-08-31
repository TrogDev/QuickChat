using MediatR;
using QuickChat.EventBus.Abstractions;
using QuickChat.Message.Application.IntegrationEvents.Events;
using QuickChat.Message.Domain.Events;

namespace QuickChat.Message.Application.DomainEventHandlers;

public class MessageEditedDomainEventHandler(IEventBus eventBus)
    : INotificationHandler<MessageEditedDomainEvent>
{
    private readonly IEventBus eventBus = eventBus;

    public Task Handle(MessageEditedDomainEvent notification, CancellationToken cancellationToken)
    {
        MessageEditedIntegrationEvent integrationEvent = new() { Message = notification.Message };
        eventBus.PublishAsync(integrationEvent);
        return Task.CompletedTask;
    }
}
