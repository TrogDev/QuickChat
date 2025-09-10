using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.Hubs;

public interface IChatHubClient
{
    Task MessageAdded(MessageModel message);
    Task MessageEdited(MessageModel message);
    Task MessageDeleted(MessageModel message);
    Task SystemMessageAdded(SystemMessageModel systemMessage);
    Task UserJoined(Guid chatId, ChatParticipantModel chatParticipant);
}
