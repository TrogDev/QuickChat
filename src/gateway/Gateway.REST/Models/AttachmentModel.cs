using QuickChat.Gateway.REST.Enums;

namespace QuickChat.Gateway.REST.Models;

public record AttachmentModel(Guid Id, string FileName, AttachmentType Type, string Url, long Size);
