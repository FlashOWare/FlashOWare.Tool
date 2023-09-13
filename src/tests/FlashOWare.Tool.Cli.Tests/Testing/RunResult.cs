namespace FlashOWare.Tool.Cli.Tests.Testing;

public sealed class RunResult
{
    private bool _isSet;

    internal RunResult()
    {
    }

    internal int ExitCode { get; private set; }

    internal void Set(int exitCode)
    {
        if (_isSet)
        {
            throw new InvalidOperationException("Result already set.");
        }

        _isSet = true;

        ExitCode = exitCode;
    }

    internal void Verify(int exitCode)
    {
        if (!_isSet)
        {
            throw new InvalidOperationException("Result not set.");
        }

        Assert.Equal(exitCode, ExitCode);
    }
}
