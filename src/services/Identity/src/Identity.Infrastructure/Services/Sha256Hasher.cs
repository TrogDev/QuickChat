using System.Security.Cryptography;
using System.Text;
using QuickChat.Identity.Application.Services;

namespace QuickChat.Identity.Infrastructure.Services;

public class Sha256Hasher : IHasher
{
    public string Hash(byte[] data)
    {
        byte[] bytes = SHA256.HashData(data);

        StringBuilder builder = new();
        foreach (byte b in bytes)
            builder.Append(b.ToString("x2"));

        return builder.ToString();
    }

    public string Hash(string data)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(data));

        StringBuilder builder = new();
        foreach (byte b in bytes)
            builder.Append(b.ToString("x2"));

        return builder.ToString();
    }
}
