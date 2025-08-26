namespace QuickChat.Attachment.Infrastructure;

public class MinioOptions
{
    public required string BucketName { get; init; }
    public required string Url { get; init; }
    public required string User { get; init; }
    public required string Password { get; init; }
}
