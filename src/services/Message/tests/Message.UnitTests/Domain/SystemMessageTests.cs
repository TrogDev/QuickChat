using QuickChat.Message.Domain.Entities;
using QuickChat.Message.Domain.Enums;
using QuickChat.Message.Domain.Events;

namespace QuickChat.Message.UnitTests.Domain;

public class SystemMessageTest
{
    private static readonly Guid chatId = new("11aa6dfb-42cd-4f93-aa6a-23d23b4baa5a");

    [Fact]
    public void Create_ReturnsSystemMessageWithAddedEvent()
    {
        // Arrange
        string text = "Test";
        SystemMessageType type = SystemMessageType.UserJoined;

        // Act
        SystemMessage message = SystemMessage.Create(chatId, text, type);

        // Assert
        Assert.Equal(text, message.Text);
        Assert.Equal(type, message.Type);
        Assert.Equal(chatId, message.ChatId);
        Assert.Single(message.DomainEvents);
        Assert.IsType<SystemMessageAddedDomainEvent>(message.DomainEvents.First());
    }
}
