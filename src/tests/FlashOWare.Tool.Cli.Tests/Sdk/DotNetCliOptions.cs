using System.Diagnostics.CodeAnalysis;

namespace FlashOWare.Tool.Cli.Tests.Sdk;

internal sealed class DotNetCliOptions
{
    public static DotNetCliOptions None { get; } = new();

    public DotNetCliOptions()
    {
    }

    public string? Name { get; init; }
    public bool NoRestore { get; init; }

    public void AddTo(ICollection<string> collection)
    {
        if (Name is not null)
        {
            collection.Add("--name");
            collection.Add(Name);
        }
        if (NoRestore)
        {
            collection.Add("--no-restore");
        }
    }

    [MemberNotNull(nameof(Name))]
    public void ThrowIfNoName()
    {
        if (Name is null)
        {
            throw new InvalidOperationException($"{nameof(DotNetCliOptions)}: {nameof(Name)} is null.");
        }
    }
}
