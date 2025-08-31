using MediatR;
using QuickChat.EventBus.Abstractions;
using QuickChat.Message.Application.IntegrationEvents.Events;
using QuickChat.Message.Domain.Events;

namespace QuickChat.Message.Application.DomainEventHandlers;

public class SystemMessageAddedDomainEventHandler(IEventBus eventBus)
    : INotificationHandler<SystemMessageAddedDomainEvent>
{
    private readonly IEventBus eventBus = eventBus;

    public Task Handle(
        SystemMessageAddedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        SystemMessageAddedIntegrationEvent integrationEvent =
            new() { Message = notification.Message };
        eventBus.PublishAsync(integrationEvent);
        return Task.CompletedTask;
    }
}
