using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuickChat.Identity.Application.Services;
using QuickChat.Identity.Infrastructure;
using QuickChat.Identity.Infrastructure.Services;

namespace QuickChat.Identity.UnitTests.Infrastructure;

public class JwtTokenServiceTests
{
    private const string privateKey =
        "MIIEpAIBAAKCAQEAoOCVENHqKLV4TFZPI22tMWAxLFGgDhmavOe0oeFw9zABxu6X2pn0qDf/7Y+/ylDRV+Zi9rIAdIz8xmghlWeCToO/B0cPjCsGRRsgBI4f780/4djLbVpO68RTBL97REzuvWD92rGL3ihVmHqz+ATnDNquLNOvs05+CpnnoPyCFo+WokT+21rqzbYA8juLWDAn230r1EBgqyeX4eWpBeFqY/SzZCc4RrANVdNtjvnwPNhP3EOusoX3/ZfsK2Q0duRjNJ8mlF4BlNHQQL/C9HVFNhsYGjZSWu87y8nMfRI1Cv6O3jvvOlnDZhIhkIphWR04aUWpsC186ogN9e/bMhyF0wIDAQABAoIBAC6eliPvKLOOTuOUOU+zdzZxQR6uMrsTSWeHn60vhsxi8Yjj2EaFScQ7oiMF/paSz+9weGozz/T2bEPUpjLE8ZpuDDwAKC2+xAzfnOolA3U63dHFQMIaIveoh0Q/dummf6KKilXHEJiayJ8so51yUkBD2Kht2aMU0mqhCeH5enstwELgUeTXmOZAkkFuNEFn7dcd9lsvbRmbvVHXY3TBykr8wa+v2tUoO7AKWxammxi2a3LGj625bcpIOYb2PWDy/4fvZoPleCq8rBGDdJPG2BDXre1yS0ZDOgM+NbNqnMDh8hZCmXwhGTDhn46lhyzg0lJckEbypR6BoHMAjS27N7ECgYEA1JAvEGTXHIvz4E3Hj2NymQkMW8O9YsJdo1h96GfEM8KXyvBSd1nHEvWK3/OZFiRV6TyPNjIVMPjgF0l81jfUcG5tcDcBWvAbU4O+fofUvZRwQDSdNdw9usJ4GvG7cytYeGOtfRT+5hXn1wf9k9BbvC8o7nKCKE8TarXhV/CxzeMCgYEAwcCNchxC9VD9o+2UwDnVweVjml22g6JmZR/9uUTsGXFofdJq7JKU5MclgsoVOjcdZuXFu2r8coEn54ygpGmV8ZbA1XqGhltRZhVIhROfH5DLT/WATIW5GJ2/fI+Bs47oyublqb8k1V+1tnENwJFnME6JGYZMjAbtEqNa4S2p61ECgYAvnCfefSetTi+Oov06waSOYHdzKlGGCFDe4Fg7MNrCWSiUnOVrtCEhvvufE7nLOtI/ToApPGiAso3GyX/7nz+m+yTqCHf2PWdxekW1o4jj+ZZ22xIHWVA+P4aVSmWY8zF6raDTLlu1f1yRTknezMFvUgTR+7Xpf2Sy9xAFGXVJzwKBgQC9zvACqRMjlntUu6vtAPua3ylLkaD6pf8f9E4w0QdnCYXvNVMQfa0lzr69uVFAXbwNEO4749x6JHM+ULMTPmjpauFwgX8GYrGEizcnP4i0eLnklEJjNOGW7vsngML0b/04wSieuaXQ7tlqaKirTQbE+TswaTPWPfbEOooObRi4cQKBgQDG47Keb1vZKIrNzi4xr/DK32f5oJMBUtyii/1GtvUN7G2fwnaCf4TzivQxCNknCCoRNpGxhzGFUK8LsTdn14ORMKmcV0UFMnwO/lKW2zCzgpmSjT6RZ8Dpl8nQEZDGULhCC8YEhYlWvPRmqUUwNg8+1THfZ9LIwYEV71klfOHFGQ==";
    private const string privateKey2 =
        "MIIEowIBAAKCAQEAygUpe03IFPKobt1zEopwA1r5KEIo2klZXHLte6KSO8aRlccdjxw4YxPJt8WrmLaG65JXRYNXxEGPipkRqYsWhmxGtMBN4lYQwHuUUG/G/3pne4sjNOYOkQVXsvQHaX+its20BrdgryHB5qh+mR4/+k4e8ykTocup8YNPxir0A8tIFBXiSQYTAcJgeNnID4wJZRiQD6IQOuVT/pBYr+xcy4OhvvYkjNRZg8ZnCkKMmYGB95OiOrQhhCpDOjmO4hZQfcqSBBUnT9DBVzzGByZKSwr0iICTN3jEjdvSdaas5USeCqrYeuYS6/5vCyLqWHNjtZKav5Zc3dnTeSyj2rR7rwIDAQABAoIBADgchPANo6RF3gilE6AzuXcFHq1S6vvuYZavCU1M02cs+STr4EbupK7IQT/89HN/RyxJQlo5Z/hH6XGqoGBCFKiTjHQJ8dgK5a5A5OoOkexF+FY1SDsaJzOak0hORXzFCdHSgs1Ww9EBBFVnuXoYD9cbObd6UkyfP/2n5lF5frtKjR70LHy76sRr2YLH8z55DtO6QfrQiT+Z25bTSrBD1pNekAGBVpVQ8vm6auyx1XotyG1Ard8k3Tx0nCsZssO2BmhngHqFONRNrBdGp2zAvIHIHyAYi9Fxb4eJ3Mwgvwaw+mKUsw8I181Q8m2DC4kgAlnttZMRppTCBl9CuRUpzWkCgYEA/GbZJtA6EKg0PNQi9D741hIXpvtIUpocKfXBDe/at8lzOpiQstfMkMKvxRPWbAdOERqnU0dxnNtJ+bCYodNBYPyzDYN7eml0b9PMBMRzmUM5haATN67yWN+d7coGQEQwBBGC/Wt+hN7wVXslHN/hJ8s2qLO6GxdlIK6Y4IWWLi0CgYEAzOZxoiidUzJhH1+OkBtG1LF0H6xmLA28MeLo82vkCdr3HHw8WVqUbik1u3G8TS5C38vZLh06uLuVOJEgW7ubM+dV62Q1xs/Lp44ssPSAheBGxGz7O4ZQQxRPWwQQqfRZ4LSUjvc3atyRN/KOgKOZJYqHYYSF5a5QbS+8RoT+FssCgYEAnNQG34AIZCsvchBZu6uZD+JkAeQmtvR8xXGmx9Md8o5gY6tCVW2S9OuTLYA6/hq33hbSmwNMS6tR1qxs2jgaann5g08MHS6DI6zVGq06KqPDjQy+hB3XBXPEfGjH44y6zPxYiMAO995SNLr0f3F4RDKXPtGVtJmfw2PZJ8C5ZO0CgYBktc+ceIH7ZoO+O+WChyWlXSNKfh4qAQZDLth47MrE3U8gPSAcex0BmFfErf5zoQ1VmohA1jUKn4iUqSBSdj+un0Hn1PVpprDexk7JGyQgqT+fREGn3DiO/+iRP8pA/s0+WQO70V73szndWecY61BW12P7VnvJjIgTYqcTskKBSQKBgHmf9OKV2LbEcg8P9ILUwxyKkfsF3nMnmMquKoxPUEalwZ8b2Bf+iHx0MqR4kpNsqEgZxd7OF3urqNTT+/NsfsE5SmZ+17zsvexNJiyvq0X4h/yXlxNR7knufH1I3iHZNZsjx5FNZ3rjj/f5G8ELAUrNPlN0MDAZN7M35rIIsRUt";
    private const string publicKeyN =
        "oOCVENHqKLV4TFZPI22tMWAxLFGgDhmavOe0oeFw9zABxu6X2pn0qDf_7Y-_ylDRV-Zi9rIAdIz8xmghlWeCToO_B0cPjCsGRRsgBI4f780_4djLbVpO68RTBL97REzuvWD92rGL3ihVmHqz-ATnDNquLNOvs05-CpnnoPyCFo-WokT-21rqzbYA8juLWDAn230r1EBgqyeX4eWpBeFqY_SzZCc4RrANVdNtjvnwPNhP3EOusoX3_ZfsK2Q0duRjNJ8mlF4BlNHQQL_C9HVFNhsYGjZSWu87y8nMfRI1Cv6O3jvvOlnDZhIhkIphWR04aUWpsC186ogN9e_bMhyF0w";
    private const string publicKeyE = "AQAB";
    private const string userId = "16cc6dfa-46cd-4f93-aa6a-23d23a0bfd3d";
    private const string userId2 = "25aa2daa-47cc-3b97-ba7a-24d25b2bfd5a";
    private const string issuer = "issuer";
    private const string audience = "audience";
    private readonly JwtTokenService tokenService;
    private readonly JwtTokenService tokenService2;

    public JwtTokenServiceTests()
    {
        IOptions<JwtOptions> options = Options.Create(
            new JwtOptions()
            {
                Issuer = issuer,
                Audience = audience,
                PrivateKey = privateKey,
                AccessLifeTimeDays = 1
            }
        );
        IOptions<JwtOptions> options2 = Options.Create(
            new JwtOptions()
            {
                Issuer = issuer,
                Audience = audience,
                PrivateKey = privateKey2,
                AccessLifeTimeDays = 1
            }
        );
        IHasher hasher = new Sha256Hasher();
        tokenService = new JwtTokenService(options, hasher);
        tokenService2 = new JwtTokenService(options2, hasher);
    }

    [Fact]
    public void CreateAccessToken_WithUserId_ReturnsCorrectHeader()
    {
        // Arrange
        Guid userId = new Guid(JwtTokenServiceTests.userId);
        JwtSecurityTokenHandler handler = new();

        // Act
        string token = tokenService.CreateAccessToken(userId);
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

        // Assert
        Assert.Equal(SecurityAlgorithms.RsaSha256, jwtToken.Header.Alg);
        Assert.Equal("JWT", jwtToken.Header.Typ);
    }

    [Fact]
    public void CreateAccessToken_WithUserId_ReturnsCorrectPayload()
    {
        // Arrange
        Guid userId = new Guid(JwtTokenServiceTests.userId);
        JwtSecurityTokenHandler handler = new();

        // Act
        string token = tokenService.CreateAccessToken(userId);
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
        string? payloadUserId = jwtToken
            .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        // Assert
        Assert.Equal(JwtTokenServiceTests.userId, payloadUserId);
        Assert.Equal(issuer, jwtToken.Payload.Iss);
        Assert.Equal(audience, jwtToken.Payload.Aud.FirstOrDefault());
    }

    [Fact]
    public void CreateAccessToken_WithTwoIdenticalPrivateKeys_ReturnsIdenticalKeyIds()
    {
        // Arrange
        Guid userId = new Guid(JwtTokenServiceTests.userId);
        Guid userId2 = new Guid(JwtTokenServiceTests.userId2);
        JwtSecurityTokenHandler handler = new();

        // Act
        string token = tokenService.CreateAccessToken(userId);
        string token2 = tokenService.CreateAccessToken(userId2);
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
        JwtSecurityToken jwtToken2 = handler.ReadJwtToken(token2);

        // Assert
        Assert.Equal(jwtToken.Header.Kid, jwtToken2.Header.Kid);
    }

    [Fact]
    public void CreateAccessToken_WithTwoDifferentPrivateKeys_ReturnsDifferentKeyIds()
    {
        // Arrange
        Guid userId = new Guid(JwtTokenServiceTests.userId);
        Guid userId2 = new Guid(JwtTokenServiceTests.userId2);
        JwtSecurityTokenHandler handler = new();

        // Act
        string token = tokenService.CreateAccessToken(userId);
        string token2 = tokenService2.CreateAccessToken(userId2); // tokenService2 uses a different private key
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
        JwtSecurityToken jwtToken2 = handler.ReadJwtToken(token2);

        // Assert
        Assert.NotEqual(jwtToken.Header.Kid, jwtToken2.Header.Kid);
    }

    [Fact]
    public void GetPublicKey_WithTwoIdenticalPrivateKeys_ReturnsIdenticalKeyIds()
    {
        // Act
        RsaSecurityKey key = tokenService.GetPublicKey();
        RsaSecurityKey key2 = tokenService.GetPublicKey();

        // Assert
        Assert.Equal(key.KeyId, key2.KeyId);
    }

    [Fact]
    public void GetPublicKey_WithTwoDifferentPrivateKeys_ReturnsDifferentKeyIds()
    {
        // Act
        RsaSecurityKey key = tokenService.GetPublicKey();
        RsaSecurityKey key2 = tokenService2.GetPublicKey(); // tokenService2 uses a different private key

        // Assert
        Assert.NotEqual(key.KeyId, key2.KeyId);
    }

    [Fact]
    public void GetPublicKey_ReturnsCorrectPublicKey()
    {
        // Act
        RsaSecurityKey key = tokenService.GetPublicKey();

        // Assert
        Assert.Equal(publicKeyN, Base64UrlEncoder.Encode(key.Parameters.Modulus));
        Assert.Equal(publicKeyE, Base64UrlEncoder.Encode(key.Parameters.Exponent));
    }
}
