using FlashOWare.Tool.Cli.Tests.Testing;
using Microsoft.CodeAnalysis.CSharp;

namespace FlashOWare.Tool.Cli.Tests.MSBuild;

internal static class MSBuildInfoExtensions
{
    public static TargetFramework GetLatestTargetFramework(this MSBuildInfo msBuild)
    {
        return msBuild.Instance.Version switch
        {
            { Major: 6 } => TargetFramework.Net60,
            { Major: 7 } => TargetFramework.Net70,
            { Major: 8 } => TargetFramework.Net80,
            _ => throw new ArgumentOutOfRangeException(nameof(msBuild), msBuild.Instance.Version, $"{nameof(Version)} not defined."),
        };
    }

    public static LanguageVersion GetLatestLanguageVersion(this MSBuildInfo msBuild)
    {
        return msBuild.Instance.Version switch
        {
            { Major: 5 } => LanguageVersion.CSharp9,
            { Major: 6 } => LanguageVersion.CSharp10,
            { Major: 7 } => LanguageVersion.CSharp11,
            { Major: 8 } => LanguageVersion.CSharp12,
            _ => throw new ArgumentOutOfRangeException(nameof(msBuild), msBuild.Instance.Version, $"{nameof(Version)} not defined."),
        };
    }
}
