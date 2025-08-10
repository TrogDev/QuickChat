using MediatR;
using QuickChat.Chat.Domain.Entities;

namespace QuickChat.Chat.Domain.Events;

public class UserJoinedChatDomainEvent : INotification
{
    public required Entities.Chat Chat { get; init; }
    public required ChatParticipant ChatParticipant { get; init; }
}
