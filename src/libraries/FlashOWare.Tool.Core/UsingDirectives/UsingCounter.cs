using FlashOWare.Tool.Core.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FlashOWare.Tool.Core.UsingDirectives;

public static class UsingCounter
{
    public static async Task<UsingCountResult> CountAsync(Project project, string[] usings, CancellationToken cancellationToken = default)
    {
        RoslynUtilities.ThrowIfNotCSharp(project);

        Compilation? compilation = await project.GetCompilationAsync(cancellationToken);
        if (compilation is null)
        {
            throw new InvalidOperationException($"{nameof(Project)}.{nameof(Project.SupportsCompilation)} = {project.SupportsCompilation} ({project.Name})");
        }

        var result = new UsingCountResult(project.Name);

        if (RoslynUtilities.IsGeneratedCode(compilation))
        {
            return result;
        }

        result.AddRange(usings);

        foreach (Document document in project.Documents)
        {
            if (RoslynUtilities.IsGeneratedCode(document))
            {
                continue;
            }

            SyntaxNode? syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
            if (syntaxRoot is null)
            {
                throw new InvalidOperationException($"{nameof(Document)}.{nameof(Document.SupportsSyntaxTree)} = {document.SupportsSyntaxTree} ({document.Name})");
            }

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

    private static void AggregateUsings(UsingCountResult result, CompilationUnitSyntax compilationUnit, string[] usings)
    {
        foreach (UsingDirectiveSyntax usingNode in compilationUnit.Usings)
        {
            if (usingNode.Alias is not null)
            {
                continue;
            }
            if (!usingNode.StaticKeyword.IsKind(SyntaxKind.None))
            {
                continue;
            }
            if (!usingNode.GlobalKeyword.IsKind(SyntaxKind.None))
            {
                continue;
            }

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
