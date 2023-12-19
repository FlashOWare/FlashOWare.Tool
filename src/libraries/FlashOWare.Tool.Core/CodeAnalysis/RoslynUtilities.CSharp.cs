using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace FlashOWare.Tool.Core.CodeAnalysis;

public static partial class RoslynUtilities
{
    internal static CompilationUnitSyntax GetCompilationUnitRoot(SyntaxTree tree, CancellationToken cancellationToken = default)
    {
        Debug.Assert(tree.HasCompilationUnitRoot, $"{nameof(SyntaxTree)}.{nameof(SyntaxTree.HasCompilationUnitRoot)} = {tree.HasCompilationUnitRoot} ({tree.FilePath})");

        return tree.GetCompilationUnitRoot(cancellationToken);
    }
}
