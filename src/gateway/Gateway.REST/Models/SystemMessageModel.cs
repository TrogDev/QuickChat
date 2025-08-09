namespace QuickChat.Gateway.REST.Models;

public record SystemMessageModel(long Id, Guid ChatId, string Text, DateTime CreatedAt);
