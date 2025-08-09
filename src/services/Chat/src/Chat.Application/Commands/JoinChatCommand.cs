using MediatR;
using QuickChat.Chat.Domain.Entities;

namespace QuickChat.Chat.Application.Commands;

public record JoinChatCommand(Guid ChatId, Guid UserId, string Name) : IRequest<ChatParticipant>;
