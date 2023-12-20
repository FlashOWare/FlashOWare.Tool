using FlashOWare.Tool.Core.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;

namespace FlashOWare.Tool.Core.UsingDirectives;

public static class UsingCounter
{
    public static Task<UsingCountResult> CountAsync(Project project, CancellationToken cancellationToken = default)
    {
        return CountAsync(project, ImmutableArray<string>.Empty, cancellationToken);
    }

    public static Task<UsingCountResult> CountAsync(Project project, string localUsing, CancellationToken cancellationToken = default)
    {
        return CountAsync(project, ImmutableArray.Create(localUsing), cancellationToken);
    }

    public static async Task<UsingCountResult> CountAsync(Project project, ImmutableArray<string> usings, CancellationToken cancellationToken = default)
    {
        RoslynUtilities.ThrowIfNotCSharp(project);

        Compilation compilation = await RoslynUtilities.GetCompilationAsync(project, cancellationToken);

        UsingCountResult result = new(project.Name);
        result.AddRange(usings);

        if (RoslynUtilities.IsGeneratedCode(compilation))
        {
            return result;
        }

        foreach (Document document in project.Documents)
        {
            if (RoslynUtilities.IsGeneratedCode(document))
            {
                continue;
            }

            SyntaxNode syntaxRoot = await RoslynUtilities.GetSyntaxRootAsync(document, cancellationToken);
            var compilationUnit = (CompilationUnitSyntax)syntaxRoot;

            RoslynUtilities.ThrowIfContainsError(compilationUnit);

            if (RoslynUtilities.IsGeneratedCode(compilationUnit))
            {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();

            AggregateUsings(result, compilationUnit, usings);
        }

        return result;
    }

    private static void AggregateUsings(UsingCountResult result, CompilationUnitSyntax compilationUnit, ImmutableArray<string> usings)
    {
        foreach (UsingDirectiveSyntax usingNode in compilationUnit.Usings)
        {
            if (usingNode.Alias is not null ||
                !usingNode.StaticKeyword.IsKind(SyntaxKind.None) ||
                !usingNode.GlobalKeyword.IsKind(SyntaxKind.None))
            {
                continue;
            }

            Debug.Assert(usingNode.Name is not null, $"Using Directive '{usingNode}' does not point at a name.");
            string identifier = usingNode.Name.ToString();

            if (usings.Length == 0)
            {
                result.IncrementOrAdd(identifier);
            }
            else if (usings.Contains(identifier))
            {
                result.Increment(identifier);
            }
        }
    }
}
