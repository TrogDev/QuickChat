using QuickChat.Message.Domain.Enums;
using QuickChat.Message.Domain.Events;

namespace QuickChat.Message.Domain.Entities;

public class SystemMessage : Entity<long>, IAggregateRoot
{
    public Guid ChatId { get; private set; }
    public string Text { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private SystemMessage() { }

    public static SystemMessage Create(Guid chatId, string text)
    {
        SystemMessage message = new() { ChatId = chatId, Text = text };
        message.AddDomainEvent(new SystemMessageAddedDomainEvent() { Message = message });
        return message;
    }
}
