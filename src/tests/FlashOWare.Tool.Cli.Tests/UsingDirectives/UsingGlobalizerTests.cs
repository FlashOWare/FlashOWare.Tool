using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.UsingDirectives;

public class UsingGlobalizerTests
{
    [Fact]
    public void Globalize_MultipleDocuments_ReplacesWithGlobalUsingDirectives()
    {
        //Arrange
        string projectPath = "../FlashOWare.Tool.ProjectUnderTest";
        //Act
        var process = Process.Start("", new[] { "flashoware" });
        //Assert
        Assert.Fail("TODO");
    }
}
