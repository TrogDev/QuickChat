using MediatR;
using QuickChat.Message.Domain.Enums;

namespace QuickChat.Message.Application.Commands;

public record AddSystemMessageCommand(Guid ChatId, string Text, SystemMessageType Type) : IRequest;
