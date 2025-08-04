namespace QuickChat.Attachment.Infrastructure;

public class MinioOptions
{
    public required string BucketName { get; init; }
    public required string Url { get; init; }
    public required string AuthenticationRegion { get; init; }
    public required string Login { get; init; }
    public required string Password { get; init; }
}
