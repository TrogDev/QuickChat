using MediatR;

namespace QuickChat.Message.Application.Commands;

/// <summary>
/// Command to delete a message by its ID.
/// </summary>
/// <param name="Id">The ID of the message to delete.</param>
/// <param name="ChatId">The chat ID the message belongs to</param>
/// <param name="ActorId">
/// The ID of the user attempting to delete the message.
/// If not null, the system will check whether the user has rights to delete the message.
/// If null, no rights check will be performed.
/// </param>
public record DeleteMessageCommand(long Id, Guid ChatId, Guid? ActorId = null) : IRequest { }
