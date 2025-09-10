namespace QuickChat.Gateway.Models;

public record ChatParticipantModel(Guid Id, Guid UserId, string Name, DateTime JoinedAt);
