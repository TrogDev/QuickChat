using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.Application.Repositories;

public interface IRepository<T>
    where T : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}
