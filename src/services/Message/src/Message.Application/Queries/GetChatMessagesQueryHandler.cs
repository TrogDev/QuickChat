using MediatR;
using QuickChat.Message.Application.Repositories;

namespace QuickChat.Message.Application.Queries;

public class GetChatMessagesQueryHandler(IMessageRepository repository)
    : IRequestHandler<GetChatMessagesQuery, IList<Domain.Entities.Message>>
{
    private readonly IMessageRepository repository = repository;

    public async Task<IList<Domain.Entities.Message>> Handle(
        GetChatMessagesQuery request,
        CancellationToken cancellationToken
    )
    {
        return await repository.GetChatMessagesAsync(request.ChatId, request.Limit, request.Cursor);
    }
}
