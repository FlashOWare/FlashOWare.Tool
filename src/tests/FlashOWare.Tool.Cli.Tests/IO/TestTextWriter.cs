using System.Diagnostics;
using System.Text;

namespace FlashOWare.Tool.Cli.Tests.IO;

internal sealed partial class TestTextWriter
{
    private readonly StringBuilder _actual = new();

    public void AssertText(string? expected)
    {
        Assert.Equal(expected, _actual.ToString());
    }
}

internal sealed partial class TestTextWriter : TextWriter
{
    public override Encoding Encoding => throw new UnreachableException();

    public override void Write(string? value)
    {
        _actual.Append(value);
    }

    public override void WriteLine()
    {
        _actual.AppendLine();
    }

    public override void WriteLine(string? value)
    {
        _actual.AppendLine(value);
    }
}
