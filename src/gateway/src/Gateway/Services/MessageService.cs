using System.Net;
using Grpc.Core;
using QuickChat.Gateway.Exceptions;
using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.Services;

public class MessageService(Message.MessageClient client, ICurrentUserProvider currentUserProvider)
    : IMessageService
{
    private readonly Message.MessageClient client = client;
    private readonly ICurrentUserProvider currentUserProvider = currentUserProvider;

    public async Task AddMessage(Guid chatId, UpdateMessageModel model)
    {
        AddMessageRequest request =
            new()
            {
                ChatId = chatId.ToString(),
                UserId = currentUserProvider.GetCurrentUserId().ToString(),
                Text = model.Text
            };
        request.AttachmentIds.AddRange(model.AttachmentIds.Select(id => id.ToString()));
        await client.AddMessageAsync(request);
    }

    public async Task DeleteMessage(Guid chatId, long messageId)
    {
        DeleteMessageRequest request =
            new()
            {
                ChatId = chatId.ToString(),
                ActorId = currentUserProvider.GetCurrentUserId().ToString(),
                Id = messageId
            };
        await client.DeleteMessageAsync(request);
    }

    public async Task EditMessage(Guid chatId, long messageId, UpdateMessageModel model)
    {
        EditMessageRequest request =
            new()
            {
                ChatId = chatId.ToString(),
                Id = messageId,
                ActorId = currentUserProvider.GetCurrentUserId().ToString(),
                Text = model.Text
            };
        request.AttachmentIds.AddRange(model.AttachmentIds.Select(id => id.ToString()));
        await client.EditMessageAsync(request);
    }

    public async Task<IList<MessageModel>> GetChatMessages(Guid chatId, int limit, long? cursor)
    {
        GetChatMessagesRequest request = new() { ChatId = chatId.ToString(), Limit = limit };

        if (cursor != null)
        {
            request.Cursor = cursor.Value;
        }

        GetChatMessagesReply reply = await client.GetChatMessagesAsync(request);
        return [.. reply.Messages.Select(MapMessage)];
    }

    private static MessageModel MapMessage(MessageMessage message)
    {
        return new MessageModel(
            message.Id,
            new Guid(message.ChatId),
            new Guid(message.UserId),
            message.Text,
            [..message.Attachments.Select(MapMessageAttachment)],
            message.CreatedAt.ToDateTime()
        );
    }

    private static MessageAttachmentModel MapMessageAttachment(MessageAttachmentMessage attachment)
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
