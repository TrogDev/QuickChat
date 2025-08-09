using MediatR;
using QuickChat.Message.Application.Repositories;

namespace QuickChat.Message.Application.Commands;

public class DeleteMessageCommandHandler(IMessageRepository repository)
    : IRequestHandler<DeleteMessageCommand>
{
    private readonly IMessageRepository repository = repository;

    public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.Message message = await repository.FindByIdAsync(
            request.ChatId,
            request.Id
        );

        if (request.ActorId == null)
        {
            message.Delete();
        }
        else
        {
            message.Delete(request.ActorId.Value);
        }

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
