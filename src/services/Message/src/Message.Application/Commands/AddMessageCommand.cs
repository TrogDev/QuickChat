using MediatR;

namespace QuickChat.Message.Application.Commands;

public record AddMessageCommand(
    Guid ChatId,
    Guid UserId,
    string Text,
    IEnumerable<Guid> AttachmentIds
) : IRequest;
