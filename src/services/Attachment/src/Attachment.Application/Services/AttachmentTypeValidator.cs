using QuickChat.Attachment.Application.Exceptions;
using QuickChat.Attachment.Domain.Enums;

namespace QuickChat.Attachment.Application.Services;

public class AttachmentTypeValidator : IAttachmentTypeValidator
{
    private static readonly HashSet<string> ImageExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

    private static readonly HashSet<string> VideoExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".mp4", ".mov", ".avi", ".mkv", ".wmv", ".webm" };

    public void Validate(AttachmentType type, string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new InvalidAttachmentTypeException();

        string extension = Path.GetExtension(fileName)?.ToLowerInvariant() ?? "";

        bool isValid = type switch
        {
            AttachmentType.Image => ImageExtensions.Contains(extension),
            AttachmentType.Video => VideoExtensions.Contains(extension),
            _ => true,
        };

        if (!isValid)
        {
            throw new InvalidAttachmentTypeException();
        }
    }
}
