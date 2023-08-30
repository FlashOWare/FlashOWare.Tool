namespace FlashOWare.Tool.Cli.Tests.Testing;

internal static class Texts
{
    public static string CreateCSharpLibraryProject(TargetFramework tfm = TargetFramework.Latest)
    {
        return $"""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFramework>{tfm.ToMonikerString()}</TargetFramework>
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
              </PropertyGroup>

            </Project>
            """;
    }

    public static string CreateCSharpLibraryProject(params TargetFramework[] tfms)
    {
        return $"""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFrameworks>{tfms.ToMonikerString()}</TargetFrameworks>
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
              </PropertyGroup>

            </Project>
            """;
    }
}
