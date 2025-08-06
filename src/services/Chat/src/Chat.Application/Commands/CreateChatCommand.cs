using MediatR;

namespace QuickChat.Chat.Application.Commands;

public record CreateChatCommand(string Name) : IRequest<Domain.Entities.Chat>;
