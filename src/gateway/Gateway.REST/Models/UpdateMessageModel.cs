namespace QuickChat.Gateway.REST.Models;

public record UpdateMessageModel(string Text, IList<Guid> AttachmentIds);
