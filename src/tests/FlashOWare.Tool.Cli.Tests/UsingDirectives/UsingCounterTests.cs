using FlashOWare.Tool.Cli.Tests.IO;
using FlashOWare.Tool.Cli.Tests.Testing;

namespace FlashOWare.Tool.Cli.Tests.UsingDirectives;

//TODO: fix WorkspaceFailed
//TODO: fix newline "issue"
//TODO: demo MSBuildWorkspace Create(IDictionary<string, string> properties)
//TODO: use System.CommandLine.IConsole
//TODO: Validate Input: [<PROJECT>]

public class UsingCounterTests : IDisposable
{
    private readonly TestTextWriter _output;

    public UsingCounterTests()
    {
        _output = new TestTextWriter();
        Console.SetOut(_output);
    }

    void IDisposable.Dispose()
    {
        _output.Dispose();
    }

    [Fact]
    public async Task Count_ProjectUnderTest_FindAllOccurrences()
    {
        //Arrange
        string[] args = CreateArgs(FileSystemUtilities.TestProject);
        //Act
        int exitCode = await CliApplication.RunAsync(args);
        //Assert
        Assert.Equal(ExitCodes.Success, exitCode);
        _output.AssertText("""
            Project: ProjectUnderTest.NetCore(net6.0)
              System: 6
              System.Collections.Generic: 6
              System.Linq: 6
              System.Text: 6
              System.Threading.Tasks: 6
              System.Reflection: 1
            """);
    }

    private static string[] CreateArgs(string project)
    {
        return new[] { "using", "count", project };
    }
}
