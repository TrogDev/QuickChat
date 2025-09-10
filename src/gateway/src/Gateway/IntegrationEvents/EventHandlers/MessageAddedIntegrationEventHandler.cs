using Microsoft.AspNetCore.SignalR;
using QuickChat.EventBus.Abstractions;
using QuickChat.Gateway.Hubs;
using QuickChat.Gateway.IntegrationEvents.Events;

namespace QuickChat.Gateway.IntegrationEvents.EventHandlers;

public class MessageAddedIntegrationEventHandler(
    IHubContext<ChatHub, IChatHubClient> hubContext,
    ILogger<MessageAddedIntegrationEventHandler> logger
) : IIntegrationEventHandler<MessageAddedIntegrationEvent>
{
    private readonly IHubContext<ChatHub, IChatHubClient> hubContext = hubContext;
    private readonly ILogger<MessageAddedIntegrationEventHandler> logger = logger;

    public async Task Handle(MessageAddedIntegrationEvent @event)
    {
        logger.LogInformation(
            "Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})",
            @event.Id,
            @event
        );

        await hubContext
            .Clients.Group(@event.Message.ChatId.ToString())
            .MessageAdded(@event.Message);
    }
}
