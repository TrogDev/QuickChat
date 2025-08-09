using QuickChat.Gateway.REST.Models;

namespace QuickChat.Gateway.REST.Services;

public interface IMessageService
{
    Task<IList<MessageModel>> GetChatMessages(Guid chatId, int limit, long? cursor);
    Task AddMessage(Guid chatId, UpdateMessageModel model);
    Task EditMessage(Guid chatId, long messageId, UpdateMessageModel model);
    Task DeleteMessage(Guid chatId, long messageId);
}
