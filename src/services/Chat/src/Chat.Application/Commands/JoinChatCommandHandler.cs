using MediatR;
using QuickChat.Chat.Application.Repositories;

namespace QuickChat.Chat.Application.Commands;

public class JoinChatCommandHandler(IChatRepository repository) : IRequestHandler<JoinChatCommand>
{
    private readonly IChatRepository repository = repository;

    public async Task Handle(JoinChatCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.Chat chat = await repository.GetById(request.ChatId);
        chat.Join(request.UserId, request.Name);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
