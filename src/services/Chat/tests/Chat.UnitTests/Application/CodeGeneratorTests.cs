using QuickChat.Chat.Application.Services;

namespace QuickChat.Chat.UnitTests.Application;

public class CodeGeneratorTests
{
    private readonly ICodeGenerator codeGenerator = new CodeGenerator();

    [Fact]
    public void Generate_WithLength8_ReturnsCodeWithLength8()
    {
        // Arrange
        int length = 8;

        // Act
        string code = codeGenerator.Generate(length);

        // Assert
        Assert.NotNull(code);
        Assert.Equal(length, code.Length);
    }
}
