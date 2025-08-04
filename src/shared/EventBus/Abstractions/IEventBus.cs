using QuickChat.EventBus.Events;

namespace QuickChat.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync(IntegrationEvent @event);
}
