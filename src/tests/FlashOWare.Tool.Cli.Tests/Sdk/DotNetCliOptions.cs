namespace FlashOWare.Tool.Cli.Tests.Sdk;

internal sealed class DotNetCliOptions
{
    public static DotNetCliOptions None { get; } = new();

    public DotNetCliOptions()
    {
    }

    public bool NoRestore { get; init; }

    public void AddTo(ICollection<string> collection)
    {
        if (NoRestore)
        {
            collection.Add("--no-restore");
        }
    }
}
