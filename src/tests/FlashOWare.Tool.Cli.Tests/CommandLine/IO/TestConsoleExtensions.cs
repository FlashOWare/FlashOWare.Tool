using System.CommandLine.IO;

namespace FlashOWare.Tool.Cli.Tests.CommandLine.IO;

internal static class TestConsoleExtensions
{
    internal static void VerifyOutput(this TestConsole console, string output)
    {
        string expected = String.Concat(output, Environment.NewLine);
        Assert.Multiple(
            () => Assert.Equal(expected, console.Out.ToString()),
            () => Assert.Empty(console.Error.ToString()!));
    }

    internal static void VerifyError(this TestConsole console, string error)
    {
        string expected = String.Concat(error, Environment.NewLine);
        Assert.Multiple(
            () => Assert.Equal(expected, console.Error.ToString()),
            () => Assert.Empty(console.Out.ToString()!));
    }
}
