using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FlashOWare.Tool.Core.CodeAnalysis;

public partial class RoslynUtilities
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
