using MediatR;
using QuickChat.Message.Application.Repositories;
using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.Application.Commands;

public class AddSystemMessageCommandHandler(ISystemMessageRepository repository)
    : IRequestHandler<AddSystemMessageCommand>
{
    private readonly ISystemMessageRepository repository = repository;

    public async Task Handle(AddSystemMessageCommand request, CancellationToken cancellationToken)
    {
        SystemMessage message = SystemMessage.Create(request.ChatId, request.Text, request.Type);
        repository.Add(message);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
