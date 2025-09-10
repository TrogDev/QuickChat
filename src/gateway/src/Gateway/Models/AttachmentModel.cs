using QuickChat.Gateway.Enums;

namespace QuickChat.Gateway.Models;

public record AttachmentModel(Guid Id, string FileName, AttachmentType Type, string Url, long Size);
