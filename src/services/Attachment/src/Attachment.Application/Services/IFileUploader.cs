using QuickChat.Attachment.Application.DTO;
using QuickChat.Attachment.Application.Exceptions;

namespace QuickChat.Attachment.Application.Services;

public interface IFileUploader
{
    /// <exception cref="FileUploadException">Thrown if an error occurs during upload.</exception>
    public Task<UploadedFileInfo> Upload(DTO.FileInfo file);
}
