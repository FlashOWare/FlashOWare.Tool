using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace FlashOWare.Tool.Core.CodeAnalysis;

public static partial class RoslynUtilities
{
    internal static void ThrowIfContainsError(Compilation compilation)
    {
        ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics();
        ThrowIfContainsError(diagnostics);
    }

    public static void ThrowIfContainsError(CompilationUnitSyntax syntaxRoot)
    {
        IEnumerable<Diagnostic> diagnostics = syntaxRoot.GetDiagnostics();
        ThrowIfContainsError(diagnostics);
    }

    private static void ThrowIfContainsError(ImmutableArray<Diagnostic> diagnostics)
    {
        if (diagnostics.IsEmpty)
        {
            return;
        }

        IEnumerable<Diagnostic> errors = diagnostics.Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);

        if (errors.Any())
        {
            ThrowErrorDiagnostics(errors.ToArray());
        }
    }

    private static void ThrowIfContainsError(IEnumerable<Diagnostic> diagnostics)
    {
        IEnumerable<Diagnostic> errors = diagnostics.Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);

        if (errors.Any())
        {
            ThrowErrorDiagnostics(errors.ToArray());
        }
    }

    private static void ThrowErrorDiagnostics(Diagnostic[] errors)
    {
        Debug.Assert(errors.Length > 0, $"No errors to be thrown.");

        var message = new StringBuilder(CreateMessage(errors))
            .AppendLine();

        foreach (Diagnostic error in errors)
        {
            Debug.Assert(error.Severity == DiagnosticSeverity.Error, $"Expected severity: {DiagnosticSeverity.Error}. Actual severity: {error.Severity}");
            message.AppendLine(error.ToString());
        }

        _ = message.Remove(message.Length - Environment.NewLine.Length, Environment.NewLine.Length);

        throw new InvalidOperationException(message.ToString());

        static string CreateMessage(Diagnostic[] diagnostics)
        {
            Debug.Assert(diagnostics.Length != 0);

            return diagnostics.Length == 1
                ? $"{nameof(Compilation)} contains an error:"
                : $"{nameof(Compilation)} contains {diagnostics.Length} errors:";
        }
    }
}
