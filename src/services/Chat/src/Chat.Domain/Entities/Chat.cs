using QuickChat.Chat.Domain.Events;
using QuickChat.Chat.Domain.Exceptions;

namespace QuickChat.Chat.Domain.Entities;

public class Chat : Entity<Guid>, IAggregateRoot
{
    public string Name { get; set; }
    public string Code { get; set; }
    public List<ChatParticipant> Participants { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long LifeTimeSeconds { get; set; } = (int)TimeSpan.FromDays(30).TotalSeconds;

    public void Join(Guid userId, string name)
    {
        if (Participants.Any(e => e.UserId == userId))
        {
            throw new UserAlreadyJoinedException();
        }

        ChatParticipant participant = new() { UserId = userId, Name = name };
        Participants.Add(participant);
        AddDomainEvent(new UserJoinedChatDomainEvent() { Chat = this, User = participant });
    }

    public bool IsExpired()
    {
        return CreatedAt.AddSeconds(LifeTimeSeconds) < DateTime.UtcNow;
    }
}
