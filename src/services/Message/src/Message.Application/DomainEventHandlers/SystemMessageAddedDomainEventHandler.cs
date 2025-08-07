using MediatR;
using QuickChat.EventBus.Abstractions;
using QuickChat.Message.Application.IntegrationEvents.Events;
using QuickChat.Message.Domain.Events;

namespace QuickChat.Message.Application.DomainEventHandlers;

public class SystemMessageAddedDomainEventHandler(IEventBus eventBus)
    : INotificationHandler<SystemMessageAddedDomainEvent>
{
    private readonly IEventBus eventBus = eventBus;

    public async Task Handle(
        SystemMessageAddedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        SystemMessageAddedIntegrationEvent integrationEvent =
            new() { Message = notification.Message };
        await eventBus.PublishAsync(integrationEvent);
    }
}
