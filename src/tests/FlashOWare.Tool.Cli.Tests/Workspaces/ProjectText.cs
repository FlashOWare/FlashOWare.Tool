using FlashOWare.Tool.Cli.Tests.Packaging;
using FlashOWare.Tool.Cli.Tests.Testing;
using Microsoft.CodeAnalysis.CSharp;
using NuGet.Packaging;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;

namespace FlashOWare.Tool.Cli.Tests.Workspaces;

internal static partial class ProjectText
{
    public static string Create(TargetFramework tfm, LanguageVersion? langVersion = null, IReadOnlyCollection<PackageReference>? packages = null)
    {
        return $"""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFramework>{tfm.ToMonikerString()}</TargetFramework>
                {(langVersion.HasValue ? $"<LangVersion>{langVersion.Value.ToDisplayString()}</LangVersion>" : null)}
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
              </PropertyGroup>
            {CreatePackageItems("  ", packages)}
            </Project>
            """;
    }

    public static string Create(TargetFramework[] tfms, LanguageVersion? langVersion = null, IReadOnlyCollection<PackageReference>? packages = null)
    {
        return $"""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <TargetFrameworks>{tfms.ToMonikerString()}</TargetFrameworks>
                {(langVersion.HasValue ? $"<LangVersion>{langVersion.Value.ToDisplayString()}</LangVersion>" : null)}
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
              </PropertyGroup>
            {CreatePackageItems("  ", packages)}
            </Project>
            """;
    }

    public static string CreateNonSdk(TargetFramework targetFrameworkVersion, LanguageVersion langVersion, string[] files, IReadOnlyCollection<PackageReference>? packages = null)
    {
        return $"""
            <?xml version="1.0" encoding="utf-8"?>
            <Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
              <PropertyGroup>
                <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
                <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
                <ProjectGuid>{ProjectOptions.Default.ProjectGuid}</ProjectGuid>
                <OutputType>Library</OutputType>
                <AppDesignerFolder>Properties</AppDesignerFolder>
                <RootNamespace>{ProjectOptions.Default.RootNamespace}</RootNamespace>
                <AssemblyName>{ProjectOptions.Default.AssemblyName}</AssemblyName>
                <TargetFrameworkVersion>{targetFrameworkVersion.ToTargetFrameworkVersionString()}</TargetFrameworkVersion>
                <FileAlignment>512</FileAlignment>
                <Deterministic>true</Deterministic>
                <LangVersion>{langVersion.ToDisplayString()}</LangVersion>
              </PropertyGroup>
              <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
                <DebugSymbols>true</DebugSymbols>
                <DebugType>full</DebugType>
                <Optimize>false</Optimize>
                <OutputPath>bin\Debug\</OutputPath>
                <DefineConstants>DEBUG;TRACE</DefineConstants>
                <ErrorReport>prompt</ErrorReport>
                <WarningLevel>4</WarningLevel>
              </PropertyGroup>
              <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
                <DebugType>pdbonly</DebugType>
                <Optimize>true</Optimize>
                <OutputPath>bin\Release\</OutputPath>
                <DefineConstants>TRACE</DefineConstants>
                <ErrorReport>prompt</ErrorReport>
                <WarningLevel>4</WarningLevel>
              </PropertyGroup>
              <PropertyGroup>
                <IsPackable>false</IsPackable>
              </PropertyGroup>
              <ItemGroup>
                <Reference Include="System" />
                <Reference Include="System.Core" />
                <Reference Include="System.Xml.Linq" />
                <Reference Include="System.Data.DataSetExtensions" />
                <Reference Include="Microsoft.CSharp" />
                <Reference Include="System.Data" />
                <Reference Include="System.Net.Http" />
                <Reference Include="System.Xml" />
              </ItemGroup>
              <ItemGroup>
                {CreateCompileItems("    ", files)}
              </ItemGroup>
            {CreateFullPackageItems("  ", packages)}
              <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
            </Project>
            """;
    }

    public static string CreateVisualBasic(TargetFramework tfm, Microsoft.CodeAnalysis.VisualBasic.LanguageVersion? langVersion = null, IReadOnlyCollection<PackageReference>? packages = null)
    {
        return $"""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <RootNamespace>{ProjectOptions.Default.RootNamespace}</RootNamespace>
                <TargetFramework>{tfm.ToMonikerString()}</TargetFramework>
                {(langVersion.HasValue ? $"<LangVersion>{Microsoft.CodeAnalysis.VisualBasic.LanguageVersionFacts.ToDisplayString(langVersion.Value)}</LangVersion>" : null)}
              </PropertyGroup>
            {CreatePackageItems("  ", packages)}
            </Project>
            """;
    }
}

internal static partial class ProjectText
{
    private static string? CreatePackageItems(string tabString, IReadOnlyCollection<PackageReference>? packages)
    {
        if (packages is null || packages.Count == 0)
        {
            return null;
        }

        StringBuilder stringBuilder = new();
        using TextWriter writer = new StringWriter(stringBuilder, CultureInfo.InvariantCulture);
        using IndentedTextWriter items = new(writer, tabString);

        items.Indent++;
        items.WriteLine();
        items.WriteLine("<ItemGroup>");
        items.Indent++;
        foreach (PackageReference package in packages)
        {
            items.WriteLine(package.ToProjectString());
        }
        items.Indent--;
        items.WriteLine("</ItemGroup>");

        return stringBuilder.ToString();
    }

    private static string CreateFullPackageItems(string tabString, IReadOnlyCollection<PackageReference>? packages)
    {
        StringBuilder stringBuilder = new(tabString);
        using TextWriter writer = new StringWriter(stringBuilder, CultureInfo.InvariantCulture);
        using IndentedTextWriter items = new(writer, tabString);

        items.Indent++;
        items.WriteLine("<ItemGroup>");
        items.Indent++;
        if (packages is not null && packages.Count != 0)
        {
            packages.WriteFullProjectString(items);
        }
        items.Indent--;
        items.Write("</ItemGroup>");

        return stringBuilder.ToString();
    }

    private static string CreateCompileItems(string tabString, string[] files)
    {
        if (files.Length == 0)
        {
            throw new ArgumentException("At least 1 'Compile Item' required.", nameof(files));
        }

        StringBuilder stringBuilder = new();
        using TextWriter writer = new StringWriter(stringBuilder, CultureInfo.InvariantCulture);
        using IndentedTextWriter items = new(writer, tabString);

        items.Indent++;
        foreach (var file in files)
        {
            items.WriteLine($"""<Compile Include="{file}" />""");
        }

        return stringBuilder.ToString(0, stringBuilder.Length - Environment.NewLine.Length);
    }
}
