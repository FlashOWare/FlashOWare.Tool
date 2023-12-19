using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FlashOWare.Tool.Core.CodeAnalysis;

public static partial class RoslynUtilities
{
    public static void ThrowIfNotCSharp(Project project)
    {
        if (project.Language != LanguageNames.CSharp)
        {
            string message = $"{nameof(Project.Language)} {project.Language} is not supported.";
            throw new InvalidOperationException(message);
        }
    }

    public static void ThrowIfNotCSharp(Project project, LanguageVersion minimumLanguageVersion, string requiredFeature)
    {
        ThrowIfNotCSharp(project);

        if (project.ParseOptions is null)
        {
            string message = "The default parse options should be used.";
            throw new InvalidOperationException(message);
        }

        var options = (CSharpParseOptions)project.ParseOptions;

        if (options.LanguageVersion < minimumLanguageVersion)
        {
            string message = $"{LanguageNames.CSharp} {options.LanguageVersion.ToDisplayString()} is not supported. The feature '{requiredFeature}' requires {LanguageNames.CSharp} {minimumLanguageVersion.ToDisplayString()}.";
            throw new InvalidOperationException(message);
        }
    }
}

public static partial class RoslynUtilities
{
    internal static async Task<Compilation> GetCompilationAsync(Project project, CancellationToken cancellationToken = default)
    {
        Compilation? compilation = await project.GetCompilationAsync(cancellationToken);

        if (compilation is null)
        {
            throw new InvalidOperationException($"{nameof(Project)}.{nameof(Project.SupportsCompilation)} = {project.SupportsCompilation} ({project.Name})");
        }

        return compilation;
    }

    internal static async Task<SemanticModel> GetSemanticModelAsync(Document document, CancellationToken cancellationToken = default)
    {
        SemanticModel? semanticModel = await document.GetSemanticModelAsync(cancellationToken);

        if (semanticModel is null)
        {
            throw new InvalidOperationException($"{nameof(Document)}.{nameof(Document.SupportsSemanticModel)} = {document.SupportsSemanticModel} ({document.Name})");
        }

        return semanticModel;
    }

    internal static async Task<SyntaxTree> GetSyntaxTreeAsync(Document document, CancellationToken cancellationToken = default)
    {
        SyntaxTree? syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);

        if (syntaxTree is null)
        {
            throw new InvalidOperationException($"{nameof(Document)}.{nameof(Document.SupportsSyntaxTree)} = {document.SupportsSyntaxTree} ({document.Name})");
        }

        return syntaxTree;
    }

    internal static async Task<SyntaxNode> GetSyntaxRootAsync(Document document, CancellationToken cancellationToken = default)
    {
        SyntaxNode? syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);

        if (syntaxRoot is null)
        {
            throw new InvalidOperationException($"{nameof(Document)}.{nameof(Document.SupportsSyntaxTree)} = {document.SupportsSyntaxTree} ({document.Name})");
        }

        return syntaxRoot;
    }
}
