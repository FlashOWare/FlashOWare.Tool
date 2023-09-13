using System.ComponentModel;

namespace FlashOWare.Tool.Cli.Tests.Testing;

internal enum ProjectKind
{
    Classic,
    SdkStyle,
}

internal static class ProjectKindExtensions
{
    public static string ToDebugString(this ProjectKind kind)
    {
        return kind switch
        {
            ProjectKind.Classic => "Classic",
            ProjectKind.SdkStyle => "SDK-style",
            _ => throw new InvalidEnumArgumentException(nameof(kind), (int)kind, typeof(ProjectKind)),
        };
    }
}
