using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuickChat.Identity.Application.Services;

namespace QuickChat.Identity.Infrastructure.Services;

public class JwtTokenService(IOptions<JwtOptions> options, IHasher hasher) : IJwtTokenService
{
    private readonly JwtOptions options = options.Value;
    private readonly IHasher hasher = hasher;

    public string CreateAccessToken(Guid userId)
    {
        List<Claim> claims = CreateClaims(userId);
        string token = CreateToken(claims);
        return token;
    }

    private static List<Claim> CreateClaims(Guid userId)
    {
        return [new(ClaimTypes.NameIdentifier, userId.ToString())];
    }

    private string CreateToken(List<Claim> claims)
    {
        using RSA rsa = CreateRSA();
        RsaSecurityKey key = CreateRsaSecurityKey(rsa, true);
        SigningCredentials credentials = new(key, SecurityAlgorithms.RsaSha256);
        JwtSecurityToken token =
            new(
                issuer: options.Issuer,
                audience: options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(options.AccessLifeTimeDays),
                signingCredentials: credentials
            );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RsaSecurityKey GetPublicKey()
    {
        using RSA rsa = CreateRSA();
        RsaSecurityKey key = CreateRsaSecurityKey(rsa, false);
        return key;
    }

    private RSA CreateRSA()
    {
        RSA rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(
            source: Convert.FromBase64String(options.PrivateKey),
            bytesRead: out _
        );
        return rsa;
    }

    private RsaSecurityKey CreateRsaSecurityKey(RSA rsa, bool isPrivate)
    {
        string keyId = hasher.Hash(rsa.ExportSubjectPublicKeyInfo());
        return new RsaSecurityKey(rsa.ExportParameters(isPrivate)) { KeyId = keyId };
    }
}
