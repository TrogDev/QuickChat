using MediatR;

namespace QuickChat.Message.Application.Commands;

public record AddSystemMessageCommand(Guid ChatId, string Text) : IRequest;
