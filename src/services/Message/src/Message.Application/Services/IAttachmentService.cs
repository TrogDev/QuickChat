using QuickChat.Message.Application.Exceptions;
using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.Application.Services;

public interface IAttachmentService
{
    /// <exception cref="AttachmentServiceException">Thrown if an error occurs during fetch.</exception>
    Task<IList<MessageAttachment>> GetAttachments(IEnumerable<Guid> ids);
}
