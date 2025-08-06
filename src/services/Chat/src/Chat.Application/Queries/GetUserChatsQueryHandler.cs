using MediatR;
using QuickChat.Chat.Application.Repositories;

namespace QuickChat.Chat.Application.Queries;

public class GetUserChatsQueryHandler(IChatRepository repository)
    : IRequestHandler<GetUserChatsQuery, IList<Domain.Entities.Chat>>
{
    private readonly IChatRepository repository = repository;

    public async Task<IList<Domain.Entities.Chat>> Handle(
        GetUserChatsQuery request,
        CancellationToken cancellationToken
    )
    {
        return await repository.GetByUserId(request.UserId);
    }
}
