using MediatR;
using QuickChat.Message.Application.Repositories;
using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.Application.Queries;

public class GetChatSystemMessagesQueryHandler(ISystemMessageRepository repository)
    : IRequestHandler<GetChatSystemMessagesQuery, IList<SystemMessage>>
{
    private readonly ISystemMessageRepository repository = repository;

    public async Task<IList<SystemMessage>> Handle(
        GetChatSystemMessagesQuery request,
        CancellationToken cancellationToken
    )
    {
        return await repository.GetChatSystemMessagesAsync(
            request.ChatId,
            request.Limit,
            request.Cursor
        );
    }
}
