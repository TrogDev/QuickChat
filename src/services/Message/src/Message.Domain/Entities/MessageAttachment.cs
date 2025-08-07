using QuickChat.Message.Domain.Enums;

namespace QuickChat.Message.Domain.Entities;

public class MessageAttachment : Entity
{
    public Guid Id { get; set; }
    public Guid AttachmentId { get; set; }
    public string FileName { get; set; }
    public AttachmentType Type { get; set; }
    public string Url { get; set; }
    public long Size { get; set; }
}
