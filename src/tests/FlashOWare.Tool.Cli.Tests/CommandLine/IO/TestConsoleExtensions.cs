using System.CommandLine.IO;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.CommandLine.IO;

internal static class TestConsoleExtensions
{
    [Obsolete($"Specify output and/or error -or- call {nameof(VerifyEmpty)}.", true)]
    internal static void Verify(this TestConsole console)
    {
        throw new UnreachableException();
    }

    internal static void Verify(this TestConsole console, string? output = null, string? error = null)
    {
        Assert.Multiple(
            () => Assert.Equal(output ?? "", console.Out.ToString()!.TrimEnd()),
            () => Assert.Equal(error ?? "", console.Error.ToString()!.TrimEnd()));
    }

    internal static void VerifyContains(this TestConsole console, string? output = null, string? error = null)
    {
        Assert.Multiple(
            () => Assert.Contains(output ?? "", console.Out.ToString()),
            () => Assert.Contains(error ?? "", console.Error.ToString()));
    }

    internal static void VerifyEmpty(this TestConsole console)
    {
        Assert.Multiple(
            () => Assert.Empty(console.Out.ToString()!),
            () => Assert.Empty(console.Error.ToString()!));
    }

    internal static void VerifyOutput(this TestConsole console, string output)
    {
        Assert.Equal(output, console.Out.ToString()!.TrimEnd());
    }

    internal static void VerifyError(this TestConsole console, string error)
    {
        Assert.Equal(error, console.Error.ToString()!.TrimEnd());
    }
}
