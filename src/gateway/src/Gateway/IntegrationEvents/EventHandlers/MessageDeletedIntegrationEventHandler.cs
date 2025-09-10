using Microsoft.AspNetCore.SignalR;
using QuickChat.EventBus.Abstractions;
using QuickChat.Gateway.Hubs;
using QuickChat.Gateway.IntegrationEvents.Events;

namespace QuickChat.Gateway.IntegrationEvents.EventHandlers;

public class MessageDeletedIntegrationEventHandler(
    IHubContext<ChatHub, IChatHubClient> hubContext,
    ILogger<MessageDeletedIntegrationEventHandler> logger
) : IIntegrationEventHandler<MessageDeletedIntegrationEvent>
{
    private readonly IHubContext<ChatHub, IChatHubClient> hubContext = hubContext;
    private readonly ILogger<MessageDeletedIntegrationEventHandler> logger = logger;

    public async Task Handle(MessageDeletedIntegrationEvent @event)
    {
        logger.LogInformation(
            "Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})",
            @event.Id,
            @event
        );

        await hubContext
            .Clients.Group(@event.Message.ChatId.ToString())
            .MessageDeleted(@event.Message);
    }
}
