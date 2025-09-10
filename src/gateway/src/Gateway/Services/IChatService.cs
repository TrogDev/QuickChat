using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.Services;

public interface IChatService
{
    Task<IList<ChatModel>> GetCurrentUserChats();
    Task<ChatModel> CreateChat(string name);
    Task<ChatModel> GetChatByCode(string code);
    Task<ChatParticipantModel> JoinChat(Guid chatId, string name);
}
