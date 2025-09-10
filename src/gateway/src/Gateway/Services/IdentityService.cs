using Microsoft.Extensions.Options;
using QuickChat.Gateway.Models;
using QuickChat.Gateway.Options;

namespace QuickChat.Gateway.Services;

public class IdentityService(
    IServiceHttpClient client,
    IOptions<IdentityOptions> options,
    ILogger<IdentityService> logger
) : IIdentityService
{
    private readonly IServiceHttpClient client = client;
    private readonly ILogger<IdentityService> logger = logger;
    private readonly IdentityOptions options = options.Value;

    public async Task<UserCredentialsModel> CreateAnonymousUser()
    {
        logger.LogInformation("Creating an anonymous user with IdentityService");
        HttpRequestMessage request = new(HttpMethod.Post, options.Url + "/users");
        return await client.InvokeRequest<UserCredentialsModel>(request, true);
    }
}
