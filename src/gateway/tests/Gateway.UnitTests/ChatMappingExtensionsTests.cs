using Google.Protobuf.WellKnownTypes;
using QuickChat.Gateway.Extensions;
using QuickChat.Gateway.Models;
using QuickChat.Gateway.Services;

namespace QuickChat.Gateway.UnitTests;

public class ChatMappingExtensionsTests
{
    [Fact]
    public void Map_Chat_FieldsMappedCorrectly()
    {
        // Arrange
        ChatMessage chatMessage =
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Code = "12345678",
                Name = "Test",
                CreatedAt = Timestamp.FromDateTime(new DateTime(2022, 11, 11).ToUniversalTime()),
                LifeTimeSeconds = 500
            };

        // Act
        ChatModel chatModel = chatMessage.ToModel();

        // Assert
        Assert.Equal(chatMessage.Id, chatModel.Id.ToString());
        Assert.Equal(chatMessage.Code, chatModel.Code);
        Assert.Equal(chatMessage.Name, chatModel.Name);
        Assert.Equal(chatMessage.CreatedAt, Timestamp.FromDateTime(chatModel.CreatedAt));
        Assert.Equal(chatMessage.LifeTimeSeconds, chatModel.LifeTimeSeconds);
    }

    [Fact]
    public void Map_ChatParticipant_FieldsMappedCorrectly()
    {
        // Arrange
        ChatParticipantMessage chatParticipantMessage =
            new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Name = "Test",
                JoinedAt = Timestamp.FromDateTime(new DateTime(2022, 11, 11).ToUniversalTime())
            };

        // Act
        ChatParticipantModel chatParticipantModel = chatParticipantMessage.ToModel();

        // Assert
        Assert.Equal(chatParticipantMessage.Id, chatParticipantModel.Id.ToString());
        Assert.Equal(chatParticipantMessage.UserId, chatParticipantModel.UserId.ToString());
        Assert.Equal(chatParticipantMessage.Name, chatParticipantModel.Name);
        Assert.Equal(
            chatParticipantMessage.JoinedAt,
            Timestamp.FromDateTime(chatParticipantModel.JoinedAt)
        );
    }
}
