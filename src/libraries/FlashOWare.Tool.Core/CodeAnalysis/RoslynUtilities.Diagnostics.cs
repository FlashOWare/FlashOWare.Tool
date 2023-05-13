using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Text;

namespace FlashOWare.Tool.Core.CodeAnalysis;

public partial class RoslynUtilities
{
    public static void ThrowIfContainsError(CompilationUnitSyntax syntaxRoot)
    {
        var errors = syntaxRoot.GetDiagnostics()
            .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ToArray();

        if (errors.Length > 0)
        {
            var message = new StringBuilder(CreateMessage(errors))
                .AppendLine();

            foreach (var error in errors)
            {
                message.AppendLine(error.ToString());
            }

            _ = message.Remove(message.Length - Environment.NewLine.Length, Environment.NewLine.Length);

            throw new InvalidOperationException(message.ToString());
        }

        static string CreateMessage(Diagnostic[] diagnostics)
        {
            Debug.Assert(diagnostics.Length != 0);

            return diagnostics.Length == 1
                ? $"{nameof(Compilation)} contains an error:"
                : $"{nameof(Compilation)} contains {diagnostics.Length} errors:";
        }
    }
}
