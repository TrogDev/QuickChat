namespace QuickChat.Identity.Application.Services;

public interface IHasher
{
    string Hash(byte[] data);
    string Hash(string data);
}
