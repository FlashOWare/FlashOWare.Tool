using FlashOWare.Tool.Core.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace FlashOWare.Tool.Core.UsingDirectives;

public static class UsingCounter
{
    public static async Task<UsingCountResult> CountAsync(Project project, CancellationToken cancellationToken = default)
    {
        var result = new UsingCountResult();

        Compilation? compilation = await project.GetCompilationAsync(cancellationToken);
        if (compilation is null)
        {
            throw new NotSupportedException($"{nameof(Project)}.{nameof(Project.SupportsCompilation)} = {project.SupportsCompilation} ({project.Name})");
        }

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

            SyntaxNode? syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
            if (syntaxRoot is null)
            {
                throw new NotSupportedException($"{nameof(Document)}.{nameof(Document.SupportsSyntaxTree)} = {document.SupportsSyntaxTree} ({document.Name})");
            }

            var compilationUnit = (CompilationUnitSyntax)syntaxRoot;

            Verify(compilationUnit);

            if (RoslynUtilities.IsGeneratedCode(compilationUnit))
            {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();

            AggregateUsings(result, compilationUnit);
        }

        return result;
    }

    private static void AggregateUsings(UsingCountResult result, CompilationUnitSyntax compilationUnit)
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
            result.IncrementOrAdd(identifier);
        }
    }

    private static void Verify(CompilationUnitSyntax compilation)
    {
        var errors = compilation.GetDiagnostics()
            .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ToArray();

        if (errors.Length > 0)
        {
            StringBuilder test = new();
            foreach (var error in errors)
            {
                test.AppendLine(error.ToString());
            }

            throw new InvalidOperationException(test.ToString());
        }
    }
}
