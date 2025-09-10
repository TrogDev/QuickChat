using Microsoft.AspNetCore.SignalR;
using QuickChat.EventBus.Abstractions;
using QuickChat.Gateway.Hubs;
using QuickChat.Gateway.IntegrationEvents.Events;

namespace QuickChat.Gateway.IntegrationEvents.EventHandlers;

public class SystemMessageAddedIntegrationEventHandler(
    IHubContext<ChatHub, IChatHubClient> hubContext,
    ILogger<SystemMessageAddedIntegrationEventHandler> logger
) : IIntegrationEventHandler<SystemMessageAddedIntegrationEvent>
{
    private readonly IHubContext<ChatHub, IChatHubClient> hubContext = hubContext;
    private readonly ILogger<SystemMessageAddedIntegrationEventHandler> logger = logger;

    public async Task Handle(SystemMessageAddedIntegrationEvent @event)
    {
        logger.LogInformation(
            "Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})",
            @event.Id,
            @event
        );

        await hubContext
            .Clients.Group(@event.Message.ChatId.ToString())
            .SystemMessageAdded(@event.Message);
    }
}
