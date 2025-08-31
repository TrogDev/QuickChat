namespace QuickChat.Identity.Infrastructure;

public class JwtOptions
{
    /// <summary>
    /// Base URL, no trailing slash
    /// </summary>
    public required string BaseUrl { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int AccessLifeTimeDays { get; init; }
    public required string PrivateKey { get; init; }
}
