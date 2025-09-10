namespace QuickChat.Gateway.Models;

public record MessageModel(
    long Id,
    Guid ChatId,
    Guid UserId,
    string Text,
    List<MessageAttachmentModel> Attachments,
    DateTime CreatedAt
);
