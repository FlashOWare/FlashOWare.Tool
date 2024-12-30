using System.Diagnostics.CodeAnalysis;

namespace FlashOWare.Tool.Core.UsingDirectives;

public sealed class UsingCountResults
{
    private readonly List<UsingCountResult> _results;

    internal UsingCountResults()
    {
        _results = [];
    }

    [SetsRequiredMembers]
    internal UsingCountResults(string solutionName)
    {
        _results = [];
        SolutionName = solutionName;
    }

    [SetsRequiredMembers]
    public UsingCountResults(string solutionName, params UsingCountResult[] results)
    {
        _results = results.ToList();
        SolutionName = solutionName;
    }

    public required string SolutionName { get; init; }
    public IReadOnlyList<UsingCountResult> Results => _results;

    internal void Add(UsingCountResult result)
    {
        _results.Add(result);
    }

    public override string ToString()
    {
        return SolutionName;
    }
}
