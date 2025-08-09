using Google.Protobuf.WellKnownTypes;
using QuickChat.Message.API.Grpc;
using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.API.Extensions;

public static class MappingExtensions
{
    public static MessageMessage ToProto(this Domain.Entities.Message message)
    {
        MessageMessage messageProto =
            new()
            {
                Id = message.Id,
                ChatId = message.ChatId.ToString(),
                UserId = message.UserId.ToString(),
                Text = message.Text,
                CreatedAt = Timestamp.FromDateTime(message.CreatedAt)
            };
        messageProto.Attachments.AddRange(message.Attachments.Select(a => a.ToProto()));
        return messageProto;
    }

    public static MessageAttachmentMessage ToProto(this MessageAttachment attachment)
    {
        return new MessageAttachmentMessage()
        {
            Id = attachment.Id.ToString(),
            AttachmentId = attachment.AttachmentId.ToString(),
            FileName = attachment.FileName,
            Type = ToProto(attachment.Type),
            Url = attachment.Url,
            Size = attachment.Size
        };
    }

    private static AttachmentType ToProto(Domain.Enums.AttachmentType type)
    {
        return type switch
        {
            Domain.Enums.AttachmentType.Image => AttachmentType.Image,
            Domain.Enums.AttachmentType.Video => AttachmentType.Video,
            _ => AttachmentType.File,
        };
    }

    public static SystemMessageMessage ToProto(this Domain.Entities.SystemMessage message)
    {
        return new SystemMessageMessage()
        {
            Id = message.Id,
            ChatId = message.ChatId.ToString(),
            Text = message.Text,
            CreatedAt = Timestamp.FromDateTime(message.CreatedAt)
        };
    }
}
