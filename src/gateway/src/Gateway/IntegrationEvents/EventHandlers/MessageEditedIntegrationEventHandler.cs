using Microsoft.AspNetCore.SignalR;
using QuickChat.EventBus.Abstractions;
using QuickChat.Gateway.Hubs;
using QuickChat.Gateway.IntegrationEvents.Events;

namespace QuickChat.Gateway.IntegrationEvents.EventHandlers;

public class MessageEditedIntegrationEventHandler(
    IHubContext<ChatHub, IChatHubClient> hubContext,
    ILogger<MessageEditedIntegrationEventHandler> logger
) : IIntegrationEventHandler<MessageEditedIntegrationEvent>
{
    private readonly IHubContext<ChatHub, IChatHubClient> hubContext = hubContext;
    private readonly ILogger<MessageEditedIntegrationEventHandler> logger = logger;

    public async Task Handle(MessageEditedIntegrationEvent @event)
    {
        logger.LogInformation(
            "Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})",
            @event.Id,
            @event
        );

        await hubContext
            .Clients.Group(@event.Message.ChatId.ToString())
            .MessageEdited(@event.Message);
    }
}
