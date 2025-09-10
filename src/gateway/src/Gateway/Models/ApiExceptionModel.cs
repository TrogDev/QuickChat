namespace QuickChat.Gateway.Models;

public record ApiExceptionModel(int Status, string Error, string Title, string? Description = null);
