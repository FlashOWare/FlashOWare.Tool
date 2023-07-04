namespace FlashOWare.Tool.Cli.Tests.UsingDirectives;

public class UsingCounterTests
{
    [Fact(Skip = "2code ^ !2code - Episode: Integration Testing")]
    public async Task Count_ProjectUnderTest_FindAllOccurences()
    {
        //Arrange
        string projectPath = "../FlashOWare.Tool.ProjectUnderTest";
        string[] args = new[] { "using", "count", projectPath };
        //Act
        int exitCode = await CliApplication.RunAsync(args);
        //Assert
        Assert.Equal(0, exitCode);
    }
}
