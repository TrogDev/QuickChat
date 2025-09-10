namespace QuickChat.Gateway.Models;

public record UpdateMessageModel(string Text, IList<Guid> AttachmentIds);
