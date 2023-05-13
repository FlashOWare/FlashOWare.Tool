using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace FlashOWare.Tool.Core.Tests.Testing;

internal static partial class RoslynFactory
{
    public static async Task CheckAsync(Project project)
    {
        Compilation? compilation = await project.GetCompilationAsync(CancellationToken.None);
        Debug.Assert(compilation is not null, $"{nameof(Project)}.{nameof(Project.SupportsCompilation)} = {project.SupportsCompilation} ({project.Name})");

        Check(compilation);
    }

    public static void Check(Compilation compilation)
    {
        ThrowIfContainsError(compilation.GetDiagnostics());
    }

    public static void Check(Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax root)
    {
        ThrowIfContainsError(root.GetDiagnostics());
    }

    public static void Check(Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax root)
    {
        ThrowIfContainsError(root.GetDiagnostics());
    }
}

internal static partial class RoslynFactory
{
    private static void ThrowIfContainsError(ImmutableArray<Diagnostic> diagnostics)
    {
        var errors = diagnostics.Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToArray();

        ThrowIfNotEmpty(errors);
    }

    private static void ThrowIfContainsError(IEnumerable<Diagnostic> diagnostics)
    {
        var errors = diagnostics.Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToArray();

        ThrowIfNotEmpty(errors);
    }

    private static void ThrowIfNotEmpty(Diagnostic[] diagnostics)
    {
        if (diagnostics.Length > 0)
        {
            StringBuilder message = new();
            foreach (var diagnostic in diagnostics)
            {
                message.AppendLine(diagnostic.ToString());
            }

            throw new InvalidOperationException(message.ToString());
        }
    }
}
