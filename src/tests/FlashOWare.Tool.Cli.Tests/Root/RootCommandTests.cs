using FlashOWare.Tool.Cli.Tests.CommandLine.IO;
using FlashOWare.Tool.Cli.Tests.Testing;

namespace FlashOWare.Tool.Cli.Tests.Root;

public class RootCommandTests : IntegrationTests
{
    [Fact]
    public async Task RootCommand_AboutOption_PrintAbout()
    {
        //Arrange
        string[] args = { "--about" };
        //Act
        int exitCode = await RunAsync(args);
        //Assert
        Console.AssertOutput("2code ^ !2code...that is the question!");
        Assert.Equal(ExitCodes.Success, exitCode);
    }

    [Fact]
    public async Task RootCommand_InfoOption_PrintInfo()
    {
        //Arrange
        string[] args = { "--info" };
        //Act
        int exitCode = await RunAsync(args);
        //Assert
        Console.AssertOutput($"MSBuild ({MSBuild.DiscoveryType}): {MSBuild.Name} {MSBuild.Version}");
        Assert.Equal(ExitCodes.Success, exitCode);
    }
}
