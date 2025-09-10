using QuickChat.Gateway.Models;
using QuickChat.Gateway.Services;

namespace QuickChat.Gateway.Extensions;

public static class MessageMappingExtensions
{
    public static MessageModel ToModel(this MessageMessage message)
    {
        return new MessageModel(
            message.Id,
            new Guid(message.ChatId),
            new Guid(message.UserId),
            message.Text,
            [..message.Attachments.Select(a => a.ToModel())],
            message.CreatedAt.ToDateTime()
        );
    }

    public static MessageAttachmentModel ToModel(this MessageAttachmentMessage attachment)
    {
        return new MessageAttachmentModel(
            new Guid(attachment.Id),
            new Guid(attachment.AttachmentId),
            attachment.FileName,
            MapAttachmentType(attachment.Type),
            attachment.Url,
            attachment.Size
        );
    }

    private static Enums.AttachmentType MapAttachmentType(AttachmentType type)
    {
        return type switch
        {
            AttachmentType.Image => Enums.AttachmentType.Image,
            AttachmentType.Video => Enums.AttachmentType.Video,
            _ => Enums.AttachmentType.File
        };
    }
}
