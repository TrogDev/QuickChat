using MediatR;
using QuickChat.Message.Domain.Events;
using QuickChat.Message.Domain.Exceptions;

namespace QuickChat.Message.Domain.Entities;

public class Message : Entity<long>, IAggregateRoot
{
    public Guid ChatId { get; private set; }
    public Guid UserId { get; private set; }
    public string Text { get; private set; }
    public List<MessageAttachment> Attachments { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public bool IsDeleted { get; private set; }

    private Message() { }

    public static Message Create(
        Guid chatId,
        Guid userId,
        string text,
        IEnumerable<MessageAttachment> attachments
    )
    {
        Message message =
            new()
            {
                ChatId = chatId,
                UserId = userId,
                Text = text,
                Attachments = [.. attachments]
            };
        message.AddDomainEvent(new MessageAddedDomainEvent() { Message = message });
        return message;
    }

    public void Edit(Guid actorId, string text, IEnumerable<MessageAttachment> attachments)
    {
        if (actorId != UserId)
        {
            throw new ActionForbiddenException("Only the user who owns this entity can edit it");
        }

        Edit(text, attachments);
    }

    public void Edit(string text, IEnumerable<MessageAttachment> attachments)
    {
        Text = text;
        Attachments.Clear();
        Attachments.AddRange(attachments);
        AddEditedEvent();
    }

    public void Delete(Guid actorId)
    {
        if (actorId != UserId)
        {
            throw new ActionForbiddenException("Only the user who owns this entity can delete it");
        }

        Delete();
    }

    public void Delete()
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            AddDomainEvent(new MessageDeletedDomainEvent() { Message = this });
        }
    }

    private void AddEditedEvent()
    {
        INotification editedDomainEvent = DomainEvents.FirstOrDefault(
            e => e is MessageEditedDomainEvent
        );

        if (editedDomainEvent != null)
        {
            RemoveDomainEvent(editedDomainEvent);
        }

        AddDomainEvent(new MessageEditedDomainEvent() { Message = this });
    }
}
