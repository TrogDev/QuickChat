using QuickChat.Attachment.Domain.Enums;

namespace QuickChat.Attachment.Domain.Entities;

public class Attachment : IAggregateRoot
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public AttachmentType Type { get; set; }
    public string Url { get; set; }
    public long Size { get; set; }
}
