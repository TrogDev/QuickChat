using MediatR;

namespace QuickChat.Chat.Application.Queries;

public record GetChatByCodeQuery(string Code) : IRequest<Domain.Entities.Chat>;
