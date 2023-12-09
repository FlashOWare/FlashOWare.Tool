namespace FlashOWare.Tool.Cli.Tests.Sdk;

public sealed partial class DotNet
{
    private readonly DirectoryInfo _directory;

    public DotNet(DirectoryInfo directory)
    {
        _directory = directory;
    }
}
