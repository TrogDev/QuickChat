using MediatR;
using QuickChat.EventBus.Abstractions;
using QuickChat.Message.Application.IntegrationEvents.Events;
using QuickChat.Message.Domain.Events;

namespace QuickChat.Message.Application.DomainEventHandlers;

public class MessageAddedDomainEventHandler(IEventBus eventBus)
    : INotificationHandler<MessageAddedDomainEvent>
{
    private readonly IEventBus eventBus = eventBus;

    public Task Handle(MessageAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        MessageAddedIntegrationEvent integrationEvent = new() { Message = notification.Message };
        eventBus.PublishAsync(integrationEvent);
        return Task.CompletedTask;
    }
}
