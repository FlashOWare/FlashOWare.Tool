using FlashOWare.Tool.Core.CodeAnalysis;
using FlashOWare.Tool.Core.Tests.Testing;
using Microsoft.CodeAnalysis.CSharp;

namespace FlashOWare.Tool.Core.Tests.CodeAnalysis;

public partial class RoslynUtilitiesTests
{
    [Fact]
    public async Task ThrowIfNotCSharp_CSharp_DoesNotThrow()
    {
        // Arrange
        var project = await CreateProjectCheckedAsync();
        // Act
        var action = () => RoslynUtilities.ThrowIfNotCSharp(project);
        // Assert
        action.Invoke();
    }

    [Fact]
    public async Task ThrowIfNotCSharp_VisualBasic_Throws()
    {
        // Arrange
        var project = await VisualBasicFactory.CreateProjectCheckedAsync();
        // Act
        var action = () => RoslynUtilities.ThrowIfNotCSharp(project);
        // Assert
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("Language Visual Basic is not supported.", exception.Message);
    }
}

public partial class RoslynUtilitiesTests
{
    [Theory]
    [InlineData(LanguageVersion.CSharp9)]
    [InlineData(LanguageVersion.CSharp10)]
    public async Task ThrowIfNotCSharp_CSharpVersionGreaterThanOrEquals_DoesNotThrow(LanguageVersion languageVersion)
    {
        // Arrange
        var project = await ProjectBuilder.CSharp(languageVersion).BuildCheckedAsync();
        // Act
        var action = () => RoslynUtilities.ThrowIfNotCSharp(project, LanguageVersion.CSharp9, "included feature");
        // Assert
        action.Invoke();
    }

    [Theory]
    [InlineData(LanguageVersion.CSharp6, "6")]
    [InlineData(LanguageVersion.CSharp7_3, "7.3")]
    [InlineData(LanguageVersion.CSharp8, "8.0")]
    public async Task ThrowIfNotCSharp_CSharpVersionLessThan_Throws(LanguageVersion languageVersion, string display)
    {
        // Arrange
        var project = await ProjectBuilder.CSharp(languageVersion).BuildCheckedAsync();
        // Act
        var action = () => RoslynUtilities.ThrowIfNotCSharp(project, LanguageVersion.CSharp9, "included feature");
        // Assert
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal($"C# {display} is not supported. The feature 'included feature' requires C# 9.0.", exception.Message);
    }

    [Fact]
    public async Task ThrowIfNotCSharp_VisualBasicVersion_Throws()
    {
        // Arrange
        var project = await ProjectBuilder.VisualBasic().BuildCheckedAsync();
        // Act
        var action = () => RoslynUtilities.ThrowIfNotCSharp(project, LanguageVersion.CSharp9, "included feature");
        // Assert
        var exception = Assert.Throws<InvalidOperationException>(action);
        Assert.Equal("Language Visual Basic is not supported.", exception.Message);
    }
}
