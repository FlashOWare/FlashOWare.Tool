using System.Diagnostics.CodeAnalysis;

namespace FlashOWare.Tool.Core.UsingDirectives;

//TODO: de-dupe .ctor

public sealed class UsingDirective : IEquatable<UsingDirective>
{
    internal UsingDirective()
    {
    }

    [SetsRequiredMembers]
    internal UsingDirective(string name)
    {
        Name = name;
        Occurrences = 1;
    }

    [SetsRequiredMembers]
    public UsingDirective(string name, int occurrences)
    {
        Name = name;
        Occurrences = occurrences;
    }

    public required string Name { get; init; }
    public int Occurrences { get; internal set; }

    bool IEquatable<UsingDirective>.Equals(UsingDirective other)
    {
        if (other == null)
        {
            return false;
        }
        else
        {
            return Name.Equals(other.Name, StringComparison.Ordinal)
                && Occurrences == other.Occurrences;
        }
    }

    public override string ToString()
    {
        return $"{Name}: {Occurrences}";
    }
}
