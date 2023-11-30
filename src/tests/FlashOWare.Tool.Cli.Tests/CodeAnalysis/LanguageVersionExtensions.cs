using FlashOWare.Tool.Cli.Tests.Testing;
using Microsoft.CodeAnalysis.CSharp;
using System.ComponentModel;

namespace FlashOWare.Tool.Cli.Tests.CodeAnalysis;

internal static class LanguageVersionExtensions
{
    public static LanguageVersion DefaultIfNull(this LanguageVersion? langVersion, TargetFramework tfm)
    {
        if (langVersion.HasValue)
        {
            return langVersion.Value;
        }

        return tfm switch
        {
            TargetFramework.Net472 => LanguageVersion.CSharp7_3,
            TargetFramework.NetStandard20 => LanguageVersion.CSharp7_3,
            TargetFramework.Net60 => LanguageVersion.CSharp10,
            TargetFramework.Net70 => LanguageVersion.CSharp11,
            TargetFramework.Net80 => (LanguageVersion)1200,
            TargetFramework.Latest => LanguageVersion.Latest,
            _ => throw new InvalidEnumArgumentException(nameof(tfm), (int)tfm, typeof(TargetFramework)),
        };
    }
}
