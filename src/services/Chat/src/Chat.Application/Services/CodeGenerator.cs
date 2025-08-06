namespace QuickChat.Chat.Application.Services;

public class CodeGenerator : ICodeGenerator
{
    // We don't use 'O' and '0' because it's easy to mix them up
    private static readonly char[] chars =
        "ABCDEFGHIJKLMNPQRSTUVWXYZabcdefghijklmnpqrstuvwxyz123456789".ToCharArray();

    public string Generate(int length)
    {
        Random random = new();
        return new string(
            Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()
        );
    }
}
