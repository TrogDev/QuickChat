using MediatR;
using QuickChat.Chat.Application.Repositories;
using QuickChat.Chat.Domain.Entities;

namespace QuickChat.Chat.Application.Commands;

public class JoinChatCommandHandler(IChatRepository repository)
    : IRequestHandler<JoinChatCommand, ChatParticipant>
{
    private readonly IChatRepository repository = repository;

    public async Task<ChatParticipant> Handle(
        JoinChatCommand request,
        CancellationToken cancellationToken
    )
    {
        Domain.Entities.Chat chat = await repository.GetById(request.ChatId);
        ChatParticipant participant = chat.Join(request.UserId, request.Name);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return participant;
    }
}
