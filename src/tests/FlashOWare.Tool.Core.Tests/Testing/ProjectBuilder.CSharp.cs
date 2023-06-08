using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FlashOWare.Tool.Core.Tests.Testing;

internal sealed partial class ProjectBuilder
{
    public static ProjectBuilder CSharp()
    {
        return CSharp(LanguageVersion.CSharp10);
    }

    public static ProjectBuilder CSharp(LanguageVersion languageVersion)
    {
        CompilationOptions compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        ParseOptions parseOptions = new CSharpParseOptions(languageVersion);

        return new ProjectBuilder(LanguageNames.CSharp, compilationOptions, parseOptions);
    }
}
