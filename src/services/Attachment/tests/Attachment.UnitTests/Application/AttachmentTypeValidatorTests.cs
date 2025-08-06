using QuickChat.Attachment.Application.Exceptions;
using QuickChat.Attachment.Application.Services;
using QuickChat.Attachment.Domain.Enums;

namespace QuickChat.Attachment.UnitTests.Application;

public class AttachmentTypeValidatorTests
{
    private readonly AttachmentTypeValidator validator = new();

    [Fact]
    public void Validate_WithInvalidImageExtension_ThrowsException()
    {
        // Arrange
        AttachmentType type = AttachmentType.Image;
        string fileName = "test.mp4";

        // Act & Assert
        Assert.Throws<InvalidAttachmentTypeException>(() => validator.Validate(type, fileName));
    }

    [Fact]
    public void Validate_WithValidImageExtension_DoesNotThrowException()
    {
        // Arrange
        AttachmentType type = AttachmentType.Image;
        string fileName = "test.jpeg";

        // Act & Assert
        validator.Validate(type, fileName);
    }

    [Fact]
    public void Validate_WithInvalidVideoExtension_ThrowsException()
    {
        // Arrange
        AttachmentType type = AttachmentType.Video;
        string fileName = "test.pdf";

        // Act & Assert
        Assert.Throws<InvalidAttachmentTypeException>(() => validator.Validate(type, fileName));
    }

    [Fact]
    public void Validate_WithValidVideoExtension_DoesNotThrowException()
    {
        // Arrange
        AttachmentType type = AttachmentType.Video;
        string fileName = "test.mp4";

        // Act & Assert
        validator.Validate(type, fileName);
    }

    [Fact]
    public void Validate_WithoutExtension_DoesNotThrowException()
    {
        // Arrange
        AttachmentType type = AttachmentType.File;
        string fileName = "test";

        // Act & Assert
        validator.Validate(type, fileName);
    }
}
