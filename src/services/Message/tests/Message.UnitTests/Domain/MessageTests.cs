using QuickChat.Message.Domain.Events;
using QuickChat.Message.Domain.Exceptions;
using Entities = QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.UnitTests.Domain;

public class MessageTest
{
    private static readonly Guid chatId = new("11aa6dfb-42cd-4f93-aa6a-23d23b4baa5a");
    private static readonly Guid userId = new("16cc6dfa-46cd-4f93-aa6a-23d23a0bfd3d");
    private static readonly Guid userId2 = new("25aa2daa-47cc-3b97-ba7a-24d25b2bfd5a");

    [Fact]
    public void Create_ReturnsMessageWithAddedEvent()
    {
        // Arrange
        string text = "Test";

        // Act
        Entities.Message message = Entities.Message.Create(chatId, userId, text, []);

        // Assert
        Assert.False(message.IsDeleted);
        Assert.Equal(text, message.Text);
        Assert.Equal(userId, message.UserId);
        Assert.Equal(chatId, message.ChatId);
        Assert.Empty(message.Attachments);
        Assert.Single(message.DomainEvents);
        Assert.IsType<MessageAddedDomainEvent>(message.DomainEvents.First());
    }

    [Fact]
    public void Edit_EditsMessageAndAddsEditedEvent()
    {
        // Arrange
        string text = "Test";
        string newText = "Test (edited)";
        Entities.Message message = Entities.Message.Create(chatId, userId, text, []);
        Entities.MessageAttachment attachment = new();

        // Act
        message.Edit(newText, [attachment]);

        // Assert
        Assert.False(message.IsDeleted);
        Assert.Equal(newText, message.Text);
        Assert.Equal(userId, message.UserId);
        Assert.Equal(chatId, message.ChatId);
        Assert.Single(message.Attachments);
        Assert.Equal(attachment, message.Attachments[0]);
        Assert.NotEmpty(message.DomainEvents);
        Assert.IsType<MessageEditedDomainEvent>(message.DomainEvents.Last());
    }

    [Fact]
    public void Edit_WithValidActor_DoesNotThrowException()
    {
        // Arrange
        string text = "Test";
        Entities.Message message = Entities.Message.Create(chatId, userId, text, []);

        // Act & Assert
        message.Edit(userId, text, []);
    }

    [Fact]
    public void Edit_WithInvalidActor_ThrowsException()
    {
        // Arrange
        string text = "Test";
        Entities.Message message = Entities.Message.Create(chatId, userId, text, []);

        // Act & Assert
        Assert.Throws<ActionForbiddenException>(() => message.Edit(userId2, text, []));
    }

    [Fact]
    public void Delete_MarksDeletedAndAddsDeletedEvent()
    {
        // Arrange
        string text = "Test";
        Entities.Message message = Entities.Message.Create(chatId, userId, text, []);

        // Act
        message.Delete();

        // Assert
        Assert.True(message.IsDeleted);
        Assert.NotEmpty(message.DomainEvents);
        Assert.IsType<MessageDeletedDomainEvent>(message.DomainEvents.Last());
    }

    [Fact]
    public void Delete_WithValidActor_DoesNotThrowException()
    {
        // Arrange
        string text = "Test";
        Entities.Message message = Entities.Message.Create(chatId, userId, text, []);

        // Act & Assert
        message.Delete(userId);
    }

    [Fact]
    public void Delete_WithInvalidActor_ThrowsException()
    {
        // Arrange
        string text = "Test";
        Entities.Message message = Entities.Message.Create(chatId, userId, text, []);

        // Act & Assert
        Assert.Throws<ActionForbiddenException>(() => message.Delete(userId2));
    }
}
