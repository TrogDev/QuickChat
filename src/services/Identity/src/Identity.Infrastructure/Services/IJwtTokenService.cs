using Microsoft.IdentityModel.Tokens;
using QuickChat.Identity.Application.Services;

namespace QuickChat.Identity.Infrastructure.Services;

public interface IJwtTokenService : ITokenService
{
    RsaSecurityKey GetPublicKey();
}
