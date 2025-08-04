using System.Text;
using QuickChat.Identity.Infrastructure.Services;

namespace QuickChat.Identity.UnitTests.Infrastructure;

public class Sha256HasherTests
{
    private readonly Sha256Hasher hasher = new Sha256Hasher();

    private const string message = "hello world";
    private const string messageHash =
        "b94d27b9934d3e08a52e52d7da7dabfac484efe37a5380ee9088f7ace2efcde9";

    [Fact]
    public void Hash_WithBytes_ReturnsCorrectHash()
    {
        // Arrange
        byte[] bytes = Encoding.UTF8.GetBytes(message);

        // Act
        string result = hasher.Hash(bytes);

        // Assert
        Assert.Equal(messageHash, result);
    }

    [Fact]
    public void Hash_WithString_ReturnsCorrectHash()
    {
        // Arrange
        byte[] bytes = Encoding.UTF8.GetBytes(message);

        // Act
        string result = hasher.Hash(bytes);

        // Assert
        Assert.Equal(messageHash, result);
    }
}
