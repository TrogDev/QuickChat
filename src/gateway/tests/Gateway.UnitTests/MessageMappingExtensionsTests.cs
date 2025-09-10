using Google.Protobuf.WellKnownTypes;
using QuickChat.Gateway.Extensions;
using QuickChat.Gateway.Models;
using QuickChat.Gateway.Services;

namespace QuickChat.Gateway.UnitTests;

public class MessageMappingExtensionsTests
{
    [Fact]
    public void Map_Message_FieldsMappedCorrectly()
    {
        // Arrange
        MessageMessage messageMessage =
            new()
            {
                Id = 1,
                ChatId = new Guid().ToString(),
                UserId = new Guid().ToString(),
                Text = "test",
                CreatedAt = Timestamp.FromDateTime(new DateTime(2022, 11, 11).ToUniversalTime())
            };

        // Act
        MessageModel messageModel = messageMessage.ToModel();

        // Assert
        Assert.Equal(messageMessage.Id, messageModel.Id);
        Assert.Equal(messageMessage.ChatId, messageMessage.ChatId.ToString());
        Assert.Equal(messageMessage.UserId, messageMessage.UserId.ToString());
        Assert.Equal(messageMessage.Text, messageModel.Text);
        Assert.Equal(messageMessage.CreatedAt, Timestamp.FromDateTime(messageModel.CreatedAt));
    }

    [Fact]
    public void Map_MessageAttachment_FieldsMappedCorrectly()
    {
        // Arrange
        MessageAttachmentMessage messageAttachmentMessage =
            new()
            {
                Id = Guid.NewGuid().ToString(),
                AttachmentId = Guid.NewGuid().ToString(),
                FileName = "Test.png",
                Type = AttachmentType.Image,
                Url = "https://example.com/",
                Size = 1000
            };

        // Act
        MessageAttachmentModel messageAttachmentModel = messageAttachmentMessage.ToModel();

        // Assert
        Assert.Equal(messageAttachmentMessage.Id, messageAttachmentModel.Id.ToString());
        Assert.Equal(
            messageAttachmentMessage.AttachmentId,
            messageAttachmentModel.AttachmentId.ToString()
        );
        Assert.Equal(messageAttachmentMessage.FileName, messageAttachmentModel.FileName);
        Assert.Equal(Enums.AttachmentType.Image, messageAttachmentModel.Type);
        Assert.Equal(messageAttachmentMessage.Url, messageAttachmentModel.Url);
        Assert.Equal(messageAttachmentMessage.Size, messageAttachmentModel.Size);
    }
}
