using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.Services;

public interface IAttachmentService
{
    Task<IList<AttachmentModel>> GetAttachments(IEnumerable<Guid> ids);
    Task<AttachmentModel> UploadAttachment(IFormFile file, Enums.AttachmentType type);
}
