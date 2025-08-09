namespace QuickChat.Gateway.REST.Models;

public record ChatModel(
    Guid Id,
    string Name,
    string Code,
    List<ChatParticipantModel> Participants,
    DateTime CreatedAt,
    long LifeTimeSeconds
) { }
