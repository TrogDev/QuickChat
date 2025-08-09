namespace QuickChat.Gateway.REST.Options;

public abstract class BaseServiceOptions
{
    /// <summary>
    /// Base URL, no trailing slash
    /// </summary>
    public required string Url { get; init; }
}
