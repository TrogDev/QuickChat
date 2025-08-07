namespace QuickChat.Message.Infrastructure;

public class AttachmentOptions
{
    /// <summary>
    /// Base URL, no trailing slash
    /// </summary>
    public required string Url { get; init; }
}
