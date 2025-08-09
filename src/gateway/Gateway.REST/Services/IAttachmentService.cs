using QuickChat.Gateway.REST.Models;

namespace QuickChat.Gateway.REST.Services;

public interface IAttachmentService
{
    Task<IList<AttachmentModel>> GetAttachments(IEnumerable<Guid> ids);
    Task<AttachmentModel> UploadAttachment(IFormFile file, Enums.AttachmentType type);
}
