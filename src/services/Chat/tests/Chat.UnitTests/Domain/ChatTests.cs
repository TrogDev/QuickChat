using QuickChat.Chat.Domain.Entities;
using QuickChat.Chat.Domain.Exceptions;

namespace QuickChat.Chat.UnitTests.Domain;

public class ChatTests
{
    private static readonly Guid userId = new("16cc6dfa-46cd-4f93-aa6a-23d23a0bfd3d");
    private static readonly Guid userId2 = new("25aa2daa-47cc-3b97-ba7a-24d25b2bfd5a");

    [Fact]
    public void Join_WithNewParticipant_AddsParticipant()
    {
        // Arrange
        Chat.Domain.Entities.Chat chat =
            new()
            {
                Id = Guid.NewGuid(),
                Code = "12345678",
                Name = "Test",
                Participants = [new ChatParticipant() { UserId = userId, Name = "Test" }]
            };

        // Act
        chat.Join(userId2, "Test");
        ChatParticipant? newParticipant = chat.Participants.FirstOrDefault(
            p => p.UserId == userId2
        );

        // Assert
        Assert.NotNull(newParticipant);
        Assert.Equal(2, chat.Participants.Count);
        Assert.Equal(userId2, newParticipant.UserId);
        Assert.Equal("Test", newParticipant.Name);
    }

    [Fact]
    public void Join_WithExistingParticipant_ThrowsException()
    {
        // Arrange
        Chat.Domain.Entities.Chat chat =
            new()
            {
                Id = Guid.NewGuid(),
                Code = "12345678",
                Name = "Test",
                Participants = [new ChatParticipant() { UserId = userId, Name = "Test" }]
            };

        // Act & Assert
        Assert.Throws<UserAlreadyJoinedException>(() =>
        {
            chat.Join(userId, "Test");
        });
    }

    [Fact]
    public void IsExpired_WithExpired_ReturnsTrue()
    {
        // Arrange
        Chat.Domain.Entities.Chat chat =
            new()
            {
                Id = Guid.NewGuid(),
                Code = "12345678",
                Name = "Test",
                Participants = [new ChatParticipant() { UserId = userId, Name = "Test" }],
            };
        chat.CreatedAt = chat.CreatedAt.AddSeconds(-chat.LifeTimeSeconds - 1);

        // Act
        bool isExpired = chat.IsExpired();

        // Assert
        Assert.True(isExpired);
    }

    [Fact]
    public void IsExpired_WithNotExpired_ReturnsFalse()
    {
        // Arrange
        Chat.Domain.Entities.Chat chat =
            new()
            {
                Id = Guid.NewGuid(),
                Code = "12345678",
                Name = "Test",
                Participants = [new ChatParticipant() { UserId = userId, Name = "Test" }],
            };

        // Act
        bool isExpired = chat.IsExpired();

        // Assert
        Assert.False(isExpired);
    }
}
