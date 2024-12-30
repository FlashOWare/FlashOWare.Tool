using System.Diagnostics.CodeAnalysis;

namespace FlashOWare.Tool.Core.UsingDirectives;

public sealed class UsingCountResult
{
    private readonly Dictionary<string, UsingDirective> _usings;

    internal UsingCountResult()
    {
        _usings = [];
    }

    [SetsRequiredMembers]
    internal UsingCountResult(string projectName)
    {
        _usings = [];
        ProjectName = projectName;
    }

    [SetsRequiredMembers]
    public UsingCountResult(string projectName, params UsingDirective[] usings)
    {
        _usings = usings.ToDictionary(static string (UsingDirective @using) => @using.Name);
        ProjectName = projectName;
    }

    public required string ProjectName { get; init; }
    public IReadOnlyCollection<UsingDirective> Usings => _usings.Values;

    internal void Add(string identifier)
    {
        _ = _usings.TryAdd(identifier, new UsingDirective(identifier));
    }

    internal void AddRange(ImmutableArray<string> identifiers)
    {
        foreach (string identifier in identifiers)
        {
            Add(identifier);
        }
    }

    internal void Increment(string identifier)
    {
        UsingDirective usingDirective = _usings[identifier];
        usingDirective.IncrementOccurrences();
    }

    internal void IncrementOrAdd(string identifier)
    {
        if (_usings.TryGetValue(identifier, out UsingDirective? usingDirective))
        {
            usingDirective.IncrementOccurrences();
        }
        else
        {
            _usings.Add(identifier, new UsingDirective(identifier, 1));
        }
    }

    public override string ToString()
    {
        return ProjectName;
    }
}
