using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QuickChat.Identity.Infrastructure.Services;

namespace QuickChat.Identity.API.Apis;

public static class DiscoveryApi
{
    public static RouteGroupBuilder MapDiscoveryApi(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder api = app.MapGroup("/.well-known");
        api.MapGet("jwks", GetJwksData);
        return api;
    }

    public static IResult GetJwksData([FromServices] IJwtTokenService tokenService)
    {
        RsaSecurityKey rsaKey = tokenService.GetPublicKey();
        RSAParameters rsaParams = rsaKey.Parameters;
        var jwk = new
        {
            kty = "RSA",
            use = "sig",
            alg = "RS256",
            kid = rsaKey.KeyId ?? Guid.NewGuid().ToString(),
            n = Base64UrlEncoder.Encode(rsaParams.Modulus),
            e = Base64UrlEncoder.Encode(rsaParams.Exponent)
        };
        return Results.Ok(new { keys = new[] { jwk } });
    }
}
