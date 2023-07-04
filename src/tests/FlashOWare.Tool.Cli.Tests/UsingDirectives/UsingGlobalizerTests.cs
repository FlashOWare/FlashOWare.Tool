namespace FlashOWare.Tool.Cli.Tests.UsingDirectives;

public class UsingGlobalizerTests
{
    [Fact(Skip = "2code ^ !2code - Episode: Integration Testing")]
    public async Task Globalize_MultipleDocuments_ReplacesWithGlobalUsingDirectives()
    {
        //Arrange
        string projectPath = "../FlashOWare.Tool.ProjectUnderTest";
        string[] args = new[] { "using", "globalize", "System", projectPath };
        //Act
        int exitCode = await CliApplication.RunAsync(args);
        //Assert
        Assert.Equal(0, exitCode);
    }
}
