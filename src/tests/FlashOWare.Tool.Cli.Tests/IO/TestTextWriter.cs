using System.Diagnostics;
using System.Text;

namespace FlashOWare.Tool.Cli.Tests.IO;

internal sealed class TestTextWriter : TextWriter
{
    private readonly StringBuilder _text = new();

    public override Encoding Encoding => throw new UnreachableException();

    public override void Write(char value)
    {
        _text.Append(value);
    }

    public override void Write(char[]? buffer)
    {
        _text.Append(buffer);
    }

    public override void Write(char[] buffer, int index, int count)
    {
        _text.Append(buffer, index, count);
    }

    public override void Write(ReadOnlySpan<char> buffer)
    {
        _text.Append(buffer);
    }

    public override void Write(bool value)
    {
        _text.Append(value);
    }

    public override void Write(int value)
    {
        _text.Append(value);
    }

    public override void Write(uint value)
    {
        _text.Append(value);
    }

    public override void Write(long value)
    {
        _text.Append(value);
    }

    public override void Write(ulong value)
    {
        _text.Append(value);
    }

    public override void Write(float value)
    {
        _text.Append(value);
    }

    public override void Write(double value)
    {
        _text.Append(value);
    }

    public override void Write(decimal value)
    {
        _text.Append(value);
    }

    public override void Write(string? value)
    {
        _text.Append(value);
    }

    public override void Write(object? value)
    {
        _text.Append(value);
    }

    public override void Write(string format, object? arg0)
    {
        _text.AppendFormat(format, arg0);
    }

    public override void Write(string format, object? arg0, object? arg1)
    {
        _text.AppendFormat(format, arg0, arg1);
    }

    public override void Write(string format, object? arg0, object? arg1, object? arg2)
    {
        _text.AppendFormat(format, arg0, arg1, arg2);
    }

    public override void Write(string format, params object?[] arg)
    {
        _text.AppendFormat(format, arg);
    }

    public override void WriteLine()
    {
        _text.AppendLine();
    }

    public override void WriteLine(char value)
    {
        _text.Append(value);
        _text.AppendLine();
    }

    public override void WriteLine(char[]? buffer)
    {
        _text.Append(buffer);
        _text.AppendLine();
    }

    public override void WriteLine(char[] buffer, int index, int count)
    {
        _text.Append(buffer, index, count);
        _text.AppendLine();
    }

    public override void WriteLine(ReadOnlySpan<char> buffer)
    {
        _text.Append(buffer);
        _text.AppendLine();
    }

    public override void WriteLine(bool value)
    {
        _text.Append(value);
        _text.AppendLine();
    }

    public override void WriteLine(int value)
    {
        _text.Append(value);
        _text.AppendLine();
    }

    public override void WriteLine(uint value)
    {
        _text.Append(value);
        _text.AppendLine();
    }

    public override void WriteLine(long value)
    {
        _text.Append(value);
        _text.AppendLine();
    }

    public override void WriteLine(ulong value)
    {
        _text.Append(value);
        _text.AppendLine();
    }

    public override void WriteLine(float value)
    {
        _text.Append(value);
        _text.AppendLine();
    }

    public override void WriteLine(double value)
    {
        _text.Append(value);
        _text.AppendLine();
    }

    public override void WriteLine(decimal value)
    {
        _text.Append(value);
        _text.AppendLine();
    }

    public override void WriteLine(string? value)
    {
        _text.AppendLine(value);
    }

    public override void WriteLine(object? value)
    {
        _text.Append(value);
        _text.AppendLine();
    }

    public override void WriteLine(string format, object? arg0)
    {
        _text.AppendFormat(format, arg0);
        _text.AppendLine();
    }

    public override void WriteLine(string format, object? arg0, object? arg1)
    {
        _text.AppendFormat(format, arg0, arg1);
        _text.AppendLine();
    }

    public override void WriteLine(string format, object? arg0, object? arg1, object? arg2)
    {
        _text.AppendFormat(format, arg0, arg1, arg2);
        _text.AppendLine();
    }

    public override void WriteLine(string format, params object?[] arg)
    {
        _text.AppendFormat(format, arg);
        _text.AppendLine();
    }

    public override string ToString()
    {
        return _text.ToString();
    }
}
