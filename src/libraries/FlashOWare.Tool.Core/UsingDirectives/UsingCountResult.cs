using System.Diagnostics.CodeAnalysis;

namespace FlashOWare.Tool.Core.UsingDirectives;

public sealed class UsingCountResult
{
    private readonly Dictionary<string, UsingDirective> _usings = new();

    internal UsingCountResult()
    {
    }

    [SetsRequiredMembers]
    internal UsingCountResult(string projectName)
    {
        ProjectName = projectName;
    }

    public required string ProjectName { get; init; }
    public IReadOnlyCollection<UsingDirective> Usings => _usings.Values;

    internal void IncrementOrAdd(string identifier)
    {
        if (_usings.TryGetValue(identifier, out UsingDirective? usingDirective))
        {
            usingDirective.Occurrences++;
        }
        else
        {
            _usings.Add(identifier, new UsingDirective(identifier));
        }
    }

    public override string ToString()
    {
        return ProjectName;
    }
}
