using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FlashOWare.Tool.Core.UsingDirectives;

//TODO: Initial Release Version (MVP)
//Ignore auto-generated documents and auto-generated code, see https://sourceroslyn.io/#Microsoft.CodeAnalysis/InternalUtilities/GeneratedCodeUtilities.cs
//check if CSharp project, throw if VisualBasic project (not supported)
//support cancellation via CancellationToken

//TODO: Return Result Type
//string ProjectName
//Occurrence
//int Occurrences

//NAME-SUGGESTIONS
//Using
//UsingDirective
//Aggregate
//Count
//Result
//Increment

public class UsingCountResult
{
    private readonly Dictionary<string, UsingOccurence> _occurences;

    public UsingCountResult()
    {
    }

    public IReadOnlyCollection<UsingOccurence> Occurences => _occurences.Values;

    public bool TryGetValue(string name, out UsingOccurence occurence)
    {
        return _occurences.TryGetValue(name, out occurence);
    }

    public void Add(string name)
    {
        _occurences.Add(name, new UsingOccurence(name));
    }
}

public class UsingOccurence
{
    public UsingOccurence()
    {
    }

    [SetsRequiredMembers]
    public UsingOccurence(string name)
    {
        Name = name;
    }

    public required string Name { get; init; }
    public int Count { get; internal set; }
    //TODO: Add flags/kinds etc. (static, global, alias)
}

public static class UsingCounter
{
    //Task<UsingCountResult>
    //Task<IEnumerable<UsingCountResult>>
    public static async Task<IReadOnlyDictionary<string, int>> CountAsync(Project project)
    {
        var result = new Dictionary<string, int>();

        foreach (Document document in project.Documents)
        {
            SyntaxNode syntaxRoot = await document.GetSyntaxRootAsync();
            var compilationUnit = (CompilationUnitSyntax)syntaxRoot;
            CountUsings(result, compilationUnit);
        }

        return result;
    }

    private static void CountUsings(Dictionary<string, int> result, CompilationUnitSyntax compilationUnit)
    {
        foreach (UsingDirectiveSyntax usingNode in compilationUnit.Usings)
        {
            if (usingNode.Alias is not null)
            {
                continue;
            }
            if (!usingNode.GlobalKeyword.IsKind(SyntaxKind.None))
            {
                continue;
            }
            if (!usingNode.StaticKeyword.IsKind(SyntaxKind.None))
            {
                continue;
            }

            string identifier = usingNode.Name.ToString();

            if (result.TryGetValue(identifier, out int count))
            {
                count++;
            }
            else
            {
                count = 1;
            }

            result[identifier] = count;
        }
    }
}
