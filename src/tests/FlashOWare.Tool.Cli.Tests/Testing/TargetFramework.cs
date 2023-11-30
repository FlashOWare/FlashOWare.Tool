using System.ComponentModel;
using System.Text;

namespace FlashOWare.Tool.Cli.Tests.Testing;

internal enum TargetFramework
{
    Net472,
    NetStandard20,
    Net60,
    Net70,
    Net80,

    Latest,
}

internal static class TargetFrameworkExtensions
{
    public static string ToMonikerString(this TargetFramework tfm)
    {
        return tfm switch
        {
            TargetFramework.Net472 => "net472",
            TargetFramework.NetStandard20 => "netstandard2.0",
            TargetFramework.Net60 => "net6.0",
            TargetFramework.Net70 => "net7.0",
            TargetFramework.Net80 => "net8.0",
            TargetFramework.Latest => "net8.0",
            _ => throw new InvalidEnumArgumentException(nameof(tfm), (int)tfm, typeof(TargetFramework)),
        };
    }

    public static string ToMonikerString(this TargetFramework[] tfms)
    {
        if (tfms.Length < 2)
        {
            throw new ArgumentException($"{nameof(tfms.Length)} must be greater than or equal to 2.", nameof(tfms));
        }

        StringBuilder moniker = new();

        for (int i = 0; i < tfms.Length; i++)
        {
            TargetFramework tfm = tfms[i];

            moniker.Append(tfm.ToMonikerString());
            moniker.Append(';');
        }

        return moniker.ToString(0, moniker.Length - 1);
    }

    public static string ToTargetFrameworkVersionString(this TargetFramework targetFrameworkVersion)
    {
        return targetFrameworkVersion switch
        {
            TargetFramework.Net472 => "v4.7.2",
            TargetFramework.NetStandard20 => throw new NotSupportedException(".NET Standard 2.0 is not supported."),
            TargetFramework.Net60 => throw new NotSupportedException(".NET 6.0 is not supported."),
            TargetFramework.Net70 => throw new NotSupportedException(".NET 7.0 is not supported."),
            TargetFramework.Net80 => throw new NotSupportedException(".NET 8.0 is not supported."),
            TargetFramework.Latest => throw new NotSupportedException($"{nameof(TargetFramework)} 'latest' is not supported."),
            _ => throw new InvalidEnumArgumentException(nameof(targetFrameworkVersion), (int)targetFrameworkVersion, typeof(TargetFramework)),
        };
    }

    public static bool IsDotNetFramework(this TargetFramework tfm)
    {
        return tfm switch
        {
            TargetFramework.Net472 => true,
            TargetFramework.NetStandard20 => false,
            TargetFramework.Net60 => false,
            TargetFramework.Net70 => false,
            TargetFramework.Net80 => false,
            TargetFramework.Latest => false,
            _ => throw new InvalidEnumArgumentException(nameof(tfm), (int)tfm, typeof(TargetFramework)),
        };
    }
}
