using QuickChat.Chat.Application.Exceptions;

namespace QuickChat.Chat.Application.Repositories;

public interface IChatRepository : IRepository<Domain.Entities.Chat>
{
    void Add(Domain.Entities.Chat chat);

    /// <summary>
    /// Retrieves a chat by its id field.
    /// Expired chats are not included.
    /// </summary>
    /// <param name="id">The chat id.</param>
    /// <returns>The chat entity matching the id.</returns>
    /// <exception cref="EntityNotFoundException">Thrown if an chat is not found.</exception>
    Task<Domain.Entities.Chat> GetById(Guid id);

    /// <summary>
    /// Retrieves a chat by its code field.
    /// Expired chats are not included.
    /// </summary>
    /// <param name="code">The chat code.</param>
    /// <returns>The chat entity matching the code.</returns>
    /// <exception cref="EntityNotFoundException">Thrown if an chat is not found.</exception>
    Task<Domain.Entities.Chat> GetByCode(string code);

    /// <summary>
    /// Retrieves all chats associated with a specific user ID.
    /// Expired chats are not included.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of chat entities for the given user ID.</returns>
    Task<IList<Domain.Entities.Chat>> GetByUserId(Guid userId);
}
