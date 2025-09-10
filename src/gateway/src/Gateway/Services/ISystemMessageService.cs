using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.Services;

public interface ISystemMessageService
{
    Task<IList<SystemMessageModel>> GetChatSystemMessages(Guid chatId, int limit, long? cursor);
}
