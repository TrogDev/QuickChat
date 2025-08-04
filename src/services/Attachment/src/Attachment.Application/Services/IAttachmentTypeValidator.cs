using QuickChat.Attachment.Application.Exceptions;
using QuickChat.Attachment.Domain.Enums;

namespace QuickChat.Attachment.Application.Services;

public interface IAttachmentTypeValidator
{
    /// <exception cref="InvalidAttachmentTypeException">Thrown if invalid.</exception>
    void Validate(AttachmentType type, string fileName);
}
