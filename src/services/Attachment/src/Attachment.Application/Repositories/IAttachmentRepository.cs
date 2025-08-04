using QuickChat.Attachment.Application.Exceptions;

namespace QuickChat.Attachment.Application.Repositories;

public interface IAttachmentRepository : IRepository<Domain.Entities.Attachment>
{
    void Add(Domain.Entities.Attachment attachment);
    Task<IList<Domain.Entities.Attachment>> FindByIdAsync(IEnumerable<Guid> ids);

    /// <exception cref="EntityNotFoundException">Thrown if an attachment is not found.</exception>
    Task<Domain.Entities.Attachment> FindByIdAsync(Guid id);
}
