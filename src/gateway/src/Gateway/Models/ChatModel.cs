namespace QuickChat.Gateway.Models;

public record ChatModel(
    Guid Id,
    string Name,
    string Code,
    List<ChatParticipantModel> Participants,
    DateTime CreatedAt,
    long LifeTimeSeconds
) { }
