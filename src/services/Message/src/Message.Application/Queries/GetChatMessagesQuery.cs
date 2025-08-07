using MediatR;

namespace QuickChat.Message.Application.Queries;

/// <summary>
/// Retrieves a list of messages from a chat, ordered from newest to oldest.
/// </summary>
/// <param name="ChatId">The unique identifier of the chat to retrieve messages from.</param>
/// <param name="Limit">The maximum number of messages to retrieve. If null, retrieves all.</param>
/// <param name="Cursor">
/// The ID of the message to start retrieving before, acting as a pagination cursor.
/// Only messages with IDs less than this value will be returned. If null, retrieval starts from the most recent message.
/// </param>
public record GetChatMessagesQuery(Guid ChatId, int? Limit = null, long? Cursor = null)
    : IRequest<IList<Domain.Entities.Message>>;
