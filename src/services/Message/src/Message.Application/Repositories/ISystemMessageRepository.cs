using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.Application.Repositories;

public interface ISystemMessageRepository : IRepository<SystemMessage>
{
    void Add(SystemMessage message);

    /// <summary>
    /// Retrieves a list of system messages from a chat, ordered from newest to oldest.
    /// </summary>
    /// <param name="chatId">The unique identifier of the chat to retrieve messages from.</param>
    /// <param name="limit">The maximum number of messages to retrieve. If null, retrieves all.</param>
    /// <param name="cursor">
    /// The ID of the message to start retrieving before, acting as a pagination cursor.
    /// Only messages with IDs less than this value will be returned. If null, retrieval starts from the most recent message.
    /// </param>
    /// <returns>List of chat system messages.</returns>
    Task<IList<SystemMessage>> GetChatSystemMessagesAsync(Guid chatId, int? limit, long? cursor);
}
