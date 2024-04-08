using NuGet.Packaging;
using System.CodeDom.Compiler;

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

    public static void WriteFullProjectString(this IReadOnlyCollection<PackageReference> packages, IndentedTextWriter textWriter)
    {
        foreach (PackageReference package in packages)
        {
            textWriter.WriteLine($"""<PackageReference Include="{package.PackageIdentity.Id}">""");
            textWriter.Indent++;

            textWriter.WriteLine($"<Version>{package.PackageIdentity.Version.ToNormalizedString()}</Version>");

            if (package.IsDevelopmentDependency)
            {
                textWriter.WriteLine("<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>");
                textWriter.WriteLine("<PrivateAssets>all</PrivateAssets>");
            }

            textWriter.Indent--;
            textWriter.WriteLine("</PackageReference>");
        }
    }
}
