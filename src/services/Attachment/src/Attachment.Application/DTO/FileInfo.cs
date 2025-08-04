namespace QuickChat.Attachment.Application.DTO;

public record FileInfo(Stream Stream, string Name, string ContentType, long Size);
