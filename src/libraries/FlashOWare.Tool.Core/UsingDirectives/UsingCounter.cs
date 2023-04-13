using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FlashOWare.Tool.Core.UsingDirectives;

//TODO: Initial Release Version, MVP
//# Count Usings
//- Top Level Usings
//- Ignore nested Usings
//- don't count static nor alias usings
//# Globalize Usings
//- create new "GlobalUsings.cs" if not existing
//- no using static
//- no using alias
//- error if exists
//- both csproj & sln

public static class UsingCounter
{
    //TODO: "largest" type: Project, don't do solution on this level
    public static IReadOnlyDictionary<string, int> Count(IEnumerable<Document> documents)
    {
        var result = new Dictionary<string, int>();

        foreach (var cSharpDocument in documents)
        {
            //TODO: change to async!
            //TODO: support cancellation via CancellationToken
            SyntaxNode syntaxRoot = cSharpDocument.GetSyntaxRootAsync().Result;
            PopulateUsings(result, syntaxRoot);
            //var newDocument = cSharpDocument.WithSyntaxRoot(null);
        }

        return result;
    }

    public static IReadOnlyDictionary<string, int> Count(IReadOnlyList<string> documents)
    {
        var result = new Dictionary<string, int>();

        foreach (string cSharpDocument in documents)
        {
            SyntaxTree cSharpSyntaxTree = CSharpSyntaxTree.ParseText(cSharpDocument);
            SyntaxNode syntaxRoot = cSharpSyntaxTree.GetRoot();
            PopulateUsings(result, syntaxRoot);
        }

        return result;
    }

    private static void PopulateUsings(Dictionary<string, int> result, SyntaxNode syntaxNode)
    {
        CompilationUnitSyntax compilationUnit = (CompilationUnitSyntax)syntaxNode;

        //TODO: ignore auto-generated code: see https://sourceroslyn.io/#Microsoft.CodeAnalysis/InternalUtilities/GeneratedCodeUtilities.cs,1a8366e77d732c39
        //TODO: consider descendIntoChildren lambda
        foreach (UsingDirectiveSyntax usingNode in syntaxNode.DescendantNodes().OfType<UsingDirectiveSyntax>())
        //foreach (UsingDirectiveSyntax usingNode in compilationUnit.Usings)
        {
            //var newNode = syntaxNode.RemoveNode(usingNode);
            //return new node
            //var newNode = usingNode.WithAlias("MyAlias");

            if (usingNode.Alias is not null)
            {
                continue;
            }
            if (usingNode.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword))
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

            if (!usingNode.StaticKeyword.IsKind(SyntaxKind.None))
            {
                identifier = $"static {identifier}";
            }

            result[identifier] = count;
        }
    }
}
