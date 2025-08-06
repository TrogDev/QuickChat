using MediatR;

namespace QuickChat.Chat.Application.Queries;

public record GetUserChatsQuery(Guid UserId) : IRequest<IList<Domain.Entities.Chat>>;
