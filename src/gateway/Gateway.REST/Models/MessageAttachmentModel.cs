using QuickChat.Gateway.REST.Enums;

namespace QuickChat.Gateway.REST.Models;

public record MessageAttachmentModel(
    Guid Id,
    Guid AttachmentId,
    string FileName,
    AttachmentType Type,
    string Url,
    long Size
);
