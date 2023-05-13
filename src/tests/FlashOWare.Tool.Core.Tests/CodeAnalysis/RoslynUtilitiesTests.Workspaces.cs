using FlashOWare.Tool.Core.CodeAnalysis;
using FlashOWare.Tool.Core.Tests.Testing;
using Microsoft.CodeAnalysis;

namespace FlashOWare.Tool.Core.Tests.CodeAnalysis;

public partial class RoslynUtilitiesTests
{
    [Fact]
    public void ThrowIfNotCSharp_CSharp_DoesNotThrow()
    {
        // Arrange
        var project = CreateProjectUnchecked();
        // Act
        var action = () => RoslynUtilities.ThrowIfNotCSharp(project);
        // Assert
        action.Invoke();
    }

    [Fact]
    public void ThrowIfNotCSharp_VisualBasic_Throws()
    {
        // Arrange
        var project = VisualBasicFactory.CreateProjectUnchecked();
        // Act
        var action = () => RoslynUtilities.ThrowIfNotCSharp(project);
        // Assert
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal($"Language {LanguageNames.VisualBasic} is not supported.", exception.Message);
    }
}
