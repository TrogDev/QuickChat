using QuickChat.Attachment.Domain.Entities;

namespace QuickChat.Attachment.Application.Repositories;

public interface IRepository<T>
    where T : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}
