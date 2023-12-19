using Microsoft.CodeAnalysis;

namespace FlashOWare.Tool.Core.CodeAnalysis;

public static partial class RoslynUtilities
{
    internal static string GetInterceptorFilePath(SyntaxTree tree, Compilation compilation)
    {
        return compilation.Options.SourceReferenceResolver?.NormalizePath(tree.FilePath, baseFilePath: null) ?? tree.FilePath;
    }
}
