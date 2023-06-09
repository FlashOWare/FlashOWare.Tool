using System.Diagnostics.CodeAnalysis;

namespace FlashOWare.Tool.Core.UsingDirectives;

public sealed partial class UsingDirective
{
    internal UsingDirective()
    {
    }

    [SetsRequiredMembers]
    internal UsingDirective(string name)
        : this(name, 0)
    {
    }

    [SetsRequiredMembers]
    public UsingDirective(string name, int occurrences)
    {
        Name = name;
        Occurrences = occurrences;
    }

    public required string Name { get; init; }
    public int Occurrences { get; internal set; }

    public override string ToString()
    {
        return $"{Name}: {Occurrences}";
    }
}
