using NuGet.Packaging;

namespace FlashOWare.Tool.Cli.Tests.Packaging;

internal static class PackageReferenceExtensions
{
    public static string ToProjectString(this PackageReference package)
    {
        if (package.IsDevelopmentDependency)
        {
            return $"""<PackageReference Include="{package.PackageIdentity.Id}" Version="{package.PackageIdentity.Version.ToNormalizedString()}" PrivateAssets="all" />""";
        }
        else
        {
            return $"""<PackageReference Include="{package.PackageIdentity.Id}" Version="{package.PackageIdentity.Version.ToNormalizedString()}" />""";
        }
    }
}
