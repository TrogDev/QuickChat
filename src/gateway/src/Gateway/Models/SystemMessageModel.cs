namespace QuickChat.Gateway.Models;

public record SystemMessageModel(long Id, Guid ChatId, string Text, DateTime CreatedAt);
