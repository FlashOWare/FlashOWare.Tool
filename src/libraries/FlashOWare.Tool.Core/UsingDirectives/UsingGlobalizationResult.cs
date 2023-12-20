using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FlashOWare.Tool.Core.UsingDirectives;

public sealed class UsingGlobalizationResult
{
    private readonly Dictionary<string, UsingDirective> _usings = [];

    internal UsingGlobalizationResult(Project project)
    {
        Project = project;
    }

    [SetsRequiredMembers]
    internal UsingGlobalizationResult(Project project, string targetDocument)
    {
        Project = project;
        TargetDocument = targetDocument;
    }

    public Project Project { get; private set; }
    public IReadOnlyCollection<UsingDirective> Usings => _usings.Values;
    public required string TargetDocument { get; init; }
    public int Occurrences { get; private set; }

    internal void Initialize(ImmutableArray<string> identifiers)
    {
        Debug.Assert(Occurrences == 0, $"Result has already been updated.");
        Debug.Assert(_usings.Count == 0, $"Result has already been initialized.");

        foreach (string identifier in identifiers)
        {
            _ = _usings.TryAdd(identifier, new UsingDirective(identifier));
        }
    }

    internal void Update(Project project)
    {
        Project = project;
    }

    internal void Update(string identifier)
    {
        if (_usings.TryGetValue(identifier, out UsingDirective? usingDirective))
        {
            usingDirective.IncrementOccurrences();
        }
        else
        {
            usingDirective = new UsingDirective(identifier, 1);
            _usings.Add(identifier, usingDirective);
        }

        Occurrences++;
    }

    internal void Update(string[] identifiers)
    {
        foreach (var identifier in identifiers)
        {
            Update(identifier);
        }
    }

    [Conditional("DEBUG")]
    internal void Verify()
    {
        int occurrences = _usings.Values.Aggregate(0, static (sum, directive) => sum + directive.Occurrences);
        Debug.Assert(occurrences == Occurrences, $"Verification of {nameof(UsingGlobalizationResult)} failed: {nameof(Occurrences)}={Occurrences} Aggregate={occurrences}");
    }

    public override string ToString()
    {
        return Project.Name;
    }
}
