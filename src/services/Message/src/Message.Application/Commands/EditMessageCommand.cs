using MediatR;

namespace QuickChat.Message.Application.Commands;

public record EditMessageCommand(
    long Id,
    Guid ChatId,
    string Text,
    IEnumerable<Guid> AttachmentIds,
    Guid? ActorId = null
) : IRequest;
