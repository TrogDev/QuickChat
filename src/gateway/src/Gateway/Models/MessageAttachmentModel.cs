using QuickChat.Gateway.Enums;

namespace QuickChat.Gateway.Models;

public record MessageAttachmentModel(
    Guid Id,
    Guid AttachmentId,
    string FileName,
    AttachmentType Type,
    string Url,
    long Size
);
