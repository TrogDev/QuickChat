using QuickChat.Gateway.REST.Models;

namespace QuickChat.Gateway.REST.Services;

public interface IChatService
{
    Task<IList<ChatModel>> GetCurrentUserChats();
    Task<ChatModel> CreateChat(string name);
    Task<ChatModel> GetChatByCode(string code);
    Task<ChatParticipantModel> JoinChat(Guid chatId, string name);
}
