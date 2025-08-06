using QuickChat.Chat.API.Grpc;
using QuickChat.Chat.Domain.Entities;

namespace QuickChat.Chat.API.Extensions;

public static class MappingExtensions
{
    public static ChatMessage ToProto(this Domain.Entities.Chat chat)
    {
        ChatMessage chatProto =
            new()
            {
                Id = chat.Id.ToString(),
                Name = chat.Name,
                Code = chat.Code
            };
        chatProto.Participants.AddRange(chat.Participants.Select(p => p.ToProto()));
        return chatProto;
    }

    public static ChatParticipantMessage ToProto(this ChatParticipant chatParticipant)
    {
        return new ChatParticipantMessage()
        {
            Id = chatParticipant.Id.ToString(),
            UserId = chatParticipant.UserId.ToString(),
            Name = chatParticipant.Name
        };
    }
}
