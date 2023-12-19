using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FlashOWare.Tool.Core.Tests.Testing;

internal sealed partial class ProjectBuilder
{
    public static ProjectBuilder CSharp()
    {
        return CSharp(LanguageVersion.CSharp10);
    }

    public static ProjectBuilder CSharp(OutputKind outputKind)
    {
        return CSharp(outputKind, LanguageVersion.CSharp10);
    }

    public static ProjectBuilder CSharp(LanguageVersion languageVersion)
    {
        return CSharp(OutputKind.DynamicallyLinkedLibrary, languageVersion);
    }

    public static ProjectBuilder CSharp(OutputKind outputKind, LanguageVersion languageVersion)
    {
        CompilationOptions compilationOptions = new CSharpCompilationOptions(outputKind);
        ParseOptions parseOptions = new CSharpParseOptions(languageVersion);

        return new ProjectBuilder(LanguageNames.CSharp, compilationOptions, parseOptions);
    }
}
