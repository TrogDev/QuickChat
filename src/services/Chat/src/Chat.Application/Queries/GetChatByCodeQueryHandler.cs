using MediatR;
using QuickChat.Chat.Application.Repositories;

namespace QuickChat.Chat.Application.Queries;

public class GetChatByCodeQueryHandler(IChatRepository repository)
    : IRequestHandler<GetChatByCodeQuery, Domain.Entities.Chat>
{
    private readonly IChatRepository repository = repository;

    public async Task<Domain.Entities.Chat> Handle(
        GetChatByCodeQuery request,
        CancellationToken cancellationToken
    )
    {
        return await repository.GetByCode(request.Code);
    }
}
