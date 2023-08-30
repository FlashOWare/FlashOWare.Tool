using System.CommandLine;
using System.CommandLine.IO;
using System.Diagnostics;
using System.Text;

namespace FlashOWare.Tool.Cli.Tests.CommandLine;

public sealed class TestConsole : IConsole
{
    private readonly TestStandardStreamWriter _out = new();
    private readonly TestStandardStreamWriter _error = new();

    public IStandardStreamWriter Out => _out;

    public bool IsOutputRedirected => throw new UnreachableException();

    public IStandardStreamWriter Error => _error;

    public bool IsErrorRedirected => throw new UnreachableException();

    public bool IsInputRedirected => throw new UnreachableException();

    internal void AssertOutput(string output)
    {
        string expected = String.Concat(output, Environment.NewLine);
        Assert.Equal(expected, _out.Text.ToString());
        Assert.Empty(_error.Text.ToString());
    }

    internal void AssertError(string error)
    {
        string expected = String.Concat(error, Environment.NewLine);
        Assert.Equal(expected, _error.Text.ToString());
        Assert.Empty(_out.Text.ToString());
    }
}

internal sealed class TestStandardStreamWriter : IStandardStreamWriter
{
    public StringBuilder Text { get; } = new();

    public void Write(string? value)
    {
        Text.Append(value);
    }
}
