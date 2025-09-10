using System.Security.Claims;
using QuickChat.Gateway.Exceptions;

namespace QuickChat.Gateway.Services;

public class CurrentUserProvider(
    IHttpContextAccessor httpContextAccessor,
    ILogger<CurrentUserProvider> logger
) : ICurrentUserProvider
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly ILogger<CurrentUserProvider> logger = logger;

    public Guid GetCurrentUserId()
    {
        string? id = httpContextAccessor
            .HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)
            ?.Value;

        if (id == null)
        {
            logger.LogError("NameIdentifier claim is missing from the current user");
            throw new MissingUserClaimException(
                "NameIdentifier claim is missing from the current user"
            );
        }

        return new Guid(id);
    }
}
