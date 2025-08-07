using MediatR;
using Microsoft.Extensions.Logging;
using QuickChat.EventBus.Abstractions;
using QuickChat.EventBus.Events;
using QuickChat.Message.Application.Commands;
using QuickChat.Message.Application.IntegrationEvents.Events;
using QuickChat.Message.Domain.Enums;

namespace QuickChat.Message.Application.IntegrationEvents.EventHandlers;

public class UserJoinedChatIntegrationEventHandler(
    IMediator mediator,
    ILogger<UserJoinedChatIntegrationEventHandler> logger
) : IIntegrationEventHandler<UserJoinedChatIntegrationEvent>
{
    private readonly IMediator mediator = mediator;
    private readonly ILogger<UserJoinedChatIntegrationEventHandler> logger = logger;

    public async Task Handle(UserJoinedChatIntegrationEvent @event)
    {
        logger.LogInformation(
            "Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})",
            @event.Id,
            @event
        );

        AddSystemMessageCommand command =
            new(
                @event.ChatId,
                $"User \"{@event.UserName}\" has joined the chat!",
                SystemMessageType.UserJoined
            );

        await mediator.Send(command);
    }
}
