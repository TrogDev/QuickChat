using QuickChat.Gateway.Models;
using QuickChat.Gateway.Services;

namespace QuickChat.Gateway.Extensions;

public static class ChatMappingExtensions
{
    public static ChatModel ToModel(this ChatMessage chat)
    {
        return new ChatModel(
            new Guid(chat.Id),
            chat.Name,
            chat.Code,
            [.. chat.Participants.Select(p => p.ToModel())],
            chat.CreatedAt.ToDateTime(),
            chat.LifeTimeSeconds
        );
    }

    public static ChatParticipantModel ToModel(this ChatParticipantMessage chatParticipant)
    {
        return new ChatParticipantModel(
            new Guid(chatParticipant.Id),
            new Guid(chatParticipant.UserId),
            chatParticipant.Name,
            chatParticipant.JoinedAt.ToDateTime()
        );
    }
}
