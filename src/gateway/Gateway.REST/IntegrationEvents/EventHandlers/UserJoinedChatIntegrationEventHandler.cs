using Microsoft.AspNetCore.SignalR;
using QuickChat.EventBus.Abstractions;
using QuickChat.Gateway.REST.Hubs;
using QuickChat.Gateway.REST.IntegrationEvents.Events;

namespace QuickChat.Gateway.REST.IntegrationEvents.EventHandlers;

public class UserJoinedChatIntegrationEventHandler(
    IHubContext<ChatHub, IChatHubClient> hubContext,
    ILogger<UserJoinedChatIntegrationEventHandler> logger
) : IIntegrationEventHandler<UserJoinedChatIntegrationEvent>
{
    private readonly IHubContext<ChatHub, IChatHubClient> hubContext = hubContext;
    private readonly ILogger<UserJoinedChatIntegrationEventHandler> logger = logger;

    public async Task Handle(UserJoinedChatIntegrationEvent @event)
    {
        logger.LogInformation(
            "Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})",
            @event.Id,
            @event
        );

        await hubContext
            .Clients.Group(@event.ChatId.ToString())
            .UserJoined(@event.ChatId, @event.ChatParticipant);
    }
}
