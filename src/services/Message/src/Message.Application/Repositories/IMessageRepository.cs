using QuickChat.Message.Application.Exceptions;

namespace QuickChat.Message.Application.Repositories;

public interface IMessageRepository : IRepository<Domain.Entities.Message>
{
    void Add(Domain.Entities.Message message);

    /// <summary>
    /// Retrieves a message by its Id field.
    /// Deleted messages are not included.
    /// </summary>
    /// <param name="id">The message id.</param>
    /// <returns>The message entity matching the id.</returns>
    /// <exception cref="EntityNotFoundException">Thrown if a message is not found.</exception>
    Task<Domain.Entities.Message> FindByIdAsync(long id);

    /// <summary>
    /// Retrieves a list of messages from a chat, ordered from newest to oldest.
    /// </summary>
    /// <param name="chatId">The unique identifier of the chat to retrieve messages from.</param>
    /// <param name="limit">The maximum number of messages to retrieve. If null, retrieves all.</param>
    /// <param name="cursor">
    /// The ID of the message to start retrieving before, acting as a pagination cursor.
    /// Only messages with IDs less than this value will be returned. If null, retrieval starts from the most recent message.
    /// </param>
    /// <returns>List of chat messages.</returns>
    Task<IList<Domain.Entities.Message>> GetChatMessagesAsync(
        Guid chatId,
        int? limit,
        long? cursor
    );
}
