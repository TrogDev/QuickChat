using QuickChat.Chat.Domain.Entities;

namespace QuickChat.Chat.Application.Repositories;

public interface IRepository<T>
    where T : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}
