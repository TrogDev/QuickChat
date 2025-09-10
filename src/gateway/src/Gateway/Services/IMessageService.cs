using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.Services;

public interface IMessageService
{
    Task<IList<MessageModel>> GetChatMessages(Guid chatId, int limit, long? cursor);
    Task AddMessage(Guid chatId, UpdateMessageModel model);
    Task EditMessage(Guid chatId, long messageId, UpdateMessageModel model);
    Task DeleteMessage(Guid chatId, long messageId);
}
