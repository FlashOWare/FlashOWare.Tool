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
        await RunAsync(args);
        //Assert
        Console.VerifyOutput("2code ^ !2code...that is the question!");
        Result.Verify(ExitCodes.Success);

    }

    [Fact]
    public async Task RootCommand_InfoOption_PrintInfo()
    {
        //Arrange
        string[] args = { "--info" };
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyOutput($"MSBuild ({MSBuild.DiscoveryType}): {MSBuild.Name} {MSBuild.Version}");
        Result.Verify(ExitCodes.Success);
    }
}
