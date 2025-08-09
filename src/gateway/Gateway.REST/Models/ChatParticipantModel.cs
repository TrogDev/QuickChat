namespace QuickChat.Gateway.REST.Models;

public record ChatParticipantModel(Guid Id, Guid UserId, string Name);
