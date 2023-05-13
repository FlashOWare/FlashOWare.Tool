using FlashOWare.Tool.Core.CodeAnalysis;

namespace FlashOWare.Tool.Core.Tests.CodeAnalysis;

public partial class RoslynUtilitiesTests
{
    [Fact]
    public void ThrowIfContainsError_NoErrors_DoesNotThrow()
    {
        // Arrange
        var compilationUnit = CreateSyntaxRootUnchecked("using System;");
        // Act
        var action = () => RoslynUtilities.ThrowIfContainsError(compilationUnit);
        // Assert
        action.Invoke();
    }

    [Fact]
    public void ThrowIfContainsError_Error_Throws()
    {
        // Arrange
        var compilationUnit = CreateSyntaxRootUnchecked("using System");
        // Act
        var action = () => RoslynUtilities.ThrowIfContainsError(compilationUnit);
        // Assert
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("""
            Compilation contains an error:
            (1,13): error CS1002: ; expected
            """, exception.Message);
    }

    [Fact]
    public void ThrowIfContainsError_Errors_Throws()
    {
        // Arrange
        var compilationUnit = CreateSyntaxRootUnchecked("using");
        // Act
        var action = () => RoslynUtilities.ThrowIfContainsError(compilationUnit);
        // Assert
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("""
            Compilation contains 2 errors:
            (1,6): error CS1001: Identifier expected
            (1,6): error CS1002: ; expected
            """, exception.Message);
    }
}
