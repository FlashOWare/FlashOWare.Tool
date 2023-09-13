using System.ComponentModel;

namespace FlashOWare.Tool.Cli.Tests.Testing;

internal enum Language
{
    CSharp,
    VisualBasic,
    FSharp,
}

internal static class LanguageExtensions
{
    public static string ToDebugString(this Language language)
    {
        return language switch
        {
            Language.CSharp => "C#",
            Language.VisualBasic => "Visual Basic",
            Language.FSharp => "F#",
            _ => throw new InvalidEnumArgumentException(nameof(language), (int)language, typeof(Language)),
        };
    }

    public static string GetDocumentExtension(this Language language, bool withLeadingPeriod = false)
    {
        return language switch
        {
            Language.CSharp => withLeadingPeriod ? ".cs" : "cs",
            Language.VisualBasic => withLeadingPeriod ? ".vb" : "vb",
            Language.FSharp => throw new NotSupportedException("F# is not a supported compile target for the Roslyn compiler."),
            _ => throw new InvalidEnumArgumentException(nameof(language), (int)language, typeof(Language)),
        };
    }

    public static string GetProjectExtension(this Language language, bool withLeadingPeriod = false)
    {
        return language switch
        {
            Language.CSharp => withLeadingPeriod ? ".csproj" : "csproj",
            Language.VisualBasic => withLeadingPeriod ? ".vbproj" : "vbproj",
            Language.FSharp => throw new NotSupportedException("F# is not a supported compile target for the Roslyn compiler."),
            _ => throw new InvalidEnumArgumentException(nameof(language), (int)language, typeof(Language)),
        };
    }
}
