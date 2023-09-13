namespace FlashOWare.Tool.Cli.Tests.IO;

internal sealed class RedirectedConsole : IDisposable
{
    private readonly TestTextWriter _out = new();
    private readonly TestTextWriter _error = new();

    public RedirectedConsole()
    {
        Console.SetOut(_out);
        Console.SetError(_error);
    }

    public void AssertEmpty()
    {
        Assert.Empty(_out.ToString());
        //Assert.Empty(_error.ToString());
    }

    public void Dispose()
    {
        _out.Dispose();
        _error.Dispose();
    }
}
