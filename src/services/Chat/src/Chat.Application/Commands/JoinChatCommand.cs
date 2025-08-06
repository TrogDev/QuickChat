using MediatR;

namespace QuickChat.Chat.Application.Commands;

public record JoinChatCommand(string Code, Guid UserId, string Name) : IRequest;
