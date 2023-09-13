using FlashOWare.Tool.Cli.Tests.Testing;
using Microsoft.CodeAnalysis.CSharp;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace FlashOWare.Tool.Cli.Tests.Workspaces;

internal static class Projects
{
    public static string CreateProject(ProjectKind kind, TargetFramework tfm, LanguageVersion langVersion, params string[] files)
    {
        return kind switch
        {
            ProjectKind.Classic => CreateNonSdkProject(tfm, langVersion, files),
            ProjectKind.SdkStyle => CreateSdkStyleProject(tfm),
            _ => throw new InvalidEnumArgumentException(nameof(kind), (int)kind, typeof(ProjectKind)),
        };
    }

    public static string CreateProject(params TargetFramework[] tfms)
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

    private static string CreateSdkStyleProject(TargetFramework tfm)
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

    private static string CreateNonSdkProject(TargetFramework targetFrameworkVersion, LanguageVersion langVersion, string[] files)
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
              <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
            </Project>
            """;

        static string CreateCompileItems(string tabString, string[] files)
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
}
