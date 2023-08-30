using System.ComponentModel;
using System.Text;

namespace FlashOWare.Tool.Cli.Tests.Testing;

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
            TargetFramework.Latest => "net7.0",
            _ => throw new InvalidEnumArgumentException(nameof(tfm), (int)tfm, typeof(TargetFramework)),
        };
    }

    public static string ToMonikerString(this TargetFramework[] tfms)
    {
        StringBuilder moniker = new();

        for (int i = 0; i < tfms.Length; i++)
        {
            TargetFramework tfm = tfms[i];

            moniker.Append(tfm.ToMonikerString());
            moniker.Append(';');
        }

        return moniker.ToString(0, moniker.Length - 1);
    }
}
