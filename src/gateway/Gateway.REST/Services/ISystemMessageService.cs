using QuickChat.Gateway.REST.Models;

namespace QuickChat.Gateway.REST.Services;

public interface ISystemMessageService
{
    Task<IList<SystemMessageModel>> GetChatSystemMessages(Guid chatId, int limit, long? cursor);
}
