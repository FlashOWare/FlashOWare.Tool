using NuGet.Frameworks;
using NuGet.Packaging;

namespace FlashOWare.Tool.Cli.Tests.Testing;

internal static class Packages
{
    public static PackageReference FlashOWare_Generators { get; } = new(new("FlashOWare.Generators", new(1, 0, 0, "prerelease.0")), NuGetFramework.AnyFramework, true, true, false);
}
