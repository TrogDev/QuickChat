using MediatR;

namespace QuickChat.Chat.Application.Commands;

public record JoinChatCommand(Guid ChatId, Guid UserId, string Name) : IRequest;
