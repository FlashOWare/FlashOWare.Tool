using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;

namespace FlashOWare.Tool.Core.Tests.Testing;

internal sealed partial class ProjectBuilder
{
    public static ProjectBuilder VisualBasic()
    {
        return VisualBasic(LanguageVersion.VisualBasic16_9);
    }

    public static ProjectBuilder VisualBasic(LanguageVersion languageVersion)
    {
        CompilationOptions compilationOptions = new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        ParseOptions parseOptions = new VisualBasicParseOptions(languageVersion);

        return new ProjectBuilder(LanguageNames.VisualBasic, compilationOptions, parseOptions);
    }
}
