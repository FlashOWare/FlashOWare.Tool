using FlashOWare.Tool.Cli.Tests.CommandLine.IO;
using FlashOWare.Tool.Cli.Tests.MSBuild;
using FlashOWare.Tool.Cli.Tests.Sdk;
using FlashOWare.Tool.Cli.Tests.Testing;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using Xunit.Abstractions;

namespace FlashOWare.Tool.Cli.Tests.Interceptors;

public class InterceptorLocatorTests : IntegrationTests
{
    private static readonly char DirectorySeparator = Path.DirectorySeparatorChar;

    public InterceptorLocatorTests(ITestOutputHelper output)
        : base(output)
    {
    }

    [Fact]
    public async Task List_WithoutInterceptors_Empty()
    {
        if (MSBuild.IsLessThan(6, 0, 100))
        {
            return;
        }

        //Arrange
        var project = Workspace.CreateProject()
            .AddDocument("""
                using System;

                namespace MyNamespace;

                public static class MyEnum
                {
                    public static void WriteIfDefined(StringComparison value)
                    {
                        if (Enum.IsDefined(value))
                        {
                            Console.WriteLine(Enum.GetName(value));
                        }
                    }
                }
                """, "MyEnum.cs")
            .Initialize(ProjectKind.SdkStyle, MSBuild.GetLatestTargetFramework(), MSBuild.GetLatestLanguageVersion());
        //Act
        await RunAsync("interceptor", "list");
        //Assert
        if (MSBuild.IsGreaterThanOrEqual(7, 0, 400))
        {
            Console.Verify($"""
                Warning: Interceptors are an experimental feature, currently available in preview mode.
                Project {project.Name} contains no interceptors.
                """, null);
            Result.Verify(ExitCodes.Success);
        }
        else
        {
            Console.Verify(null, $"""
                The 'interceptors' experimental feature is not supported in .NET SDK {MSBuild.Instance.Version}.
                """);
            Result.Verify(ExitCodes.Success);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("--proj")]
    [InlineData("--project")]
    public async Task List_WithGenerator_ListsInterceptors(string? option)
    {
        if (MSBuild.IsLessThan(8, 0, 100))
        {
            return;
        }

        //Arrange
        var project = Workspace.CreateProject()
            .AddDocument("""
                using System;

                namespace MyNamespace;

                public static class MyEnum
                {
                    public static void WriteIfDefined(StringComparison value)
                    {
                        if (Enum.IsDefined(value))
                        {
                            Console.WriteLine(Enum.GetName(value));
                        }
                    }
                }
                """, "MyEnum.cs")
            .AddPackage(Packages.FlashOWare_Generators)
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net80, LanguageVersion.CSharp12);
        Stopwatch stopwatch = Stopwatch.StartNew();
        await DotNet.RestoreAsync(project.File);
        stopwatch.Stop();
        System.Console.WriteLine($"DotNet.RestoreAsync: {stopwatch.Elapsed}");
        //Act
        await (option is null
            ? RunAsync("interceptor", "list")
            : RunAsync("interceptor", "list", option, project.File.FullName));
        //Assert
        Console.Verify($"""
            Warning: Interceptors are an experimental feature, currently available in preview mode.
            Project {project.Name} contains 2 interceptors with 2 interceptions.

            2 interceptions in MyEnum.cs
            - intercepted System.Enum.IsDefined<System.StringComparison>(System.StringComparison)
              at location MyEnum.cs:9:18
              Interceptor FlashOWare.Generated.EnumInterceptors.IsDefined0(System.StringComparison)
              at location FlashOWare.Generators{DirectorySeparator}FlashOWare.Generators.Enumerations.EnumInterceptorGenerator{DirectorySeparator}FlashOWare.Generated.EnumInterceptors.g.cs:23:24
            - intercepted System.Enum.GetName<System.StringComparison>(System.StringComparison)
              at location MyEnum.cs:11:36
              Interceptor FlashOWare.Generated.EnumInterceptors.GetName0(System.StringComparison)
              at location FlashOWare.Generators{DirectorySeparator}FlashOWare.Generators.Enumerations.EnumInterceptorGenerator{DirectorySeparator}FlashOWare.Generated.EnumInterceptors.g.cs:34:27
            """, null);
        Result.Verify(ExitCodes.Success);
    }

    [Theory]
    [InlineData("--group")]
    [InlineData("--group-by-interceptors")]
    public async Task List_GroupByInterceptors_ListsInterceptors(string option)
    {
        if (MSBuild.IsLessThan(8, 0, 100))
        {
            return;
        }

        //Arrange
        Stopwatch stopwatch = Stopwatch.StartNew();
        var project = await DotNet.NewAsync(DotNetNewTemplate.AspNetCoreWebApiNativeAot);
        stopwatch.Stop();
        System.Console.WriteLine($"DotNet.NewAsync: {stopwatch.Elapsed}");
        //Act
        await RunAsync("interceptor", "list", option);
        //Assert
        Console.Verify($"""
            Warning: Interceptors are an experimental feature, currently available in preview mode.
            Project {project} contains 2 interceptors with 2 interceptions.

            2 Interceptors
            - Interceptor: Microsoft.AspNetCore.Http.Generated.GeneratedRouteBuilderExtensionsCore.MapGet0(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder, string, System.Delegate)
              at location: Microsoft.AspNetCore.Http.RequestDelegateGenerator{DirectorySeparator}Microsoft.AspNetCore.Http.RequestDelegateGenerator.RequestDelegateGenerator{DirectorySeparator}GeneratedRouteBuilderExtensions.g.cs:62:45
              intercepts 1 method invocation:
              - Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.MapGet(string, System.Delegate)
                - Program.cs:21:10
            - Interceptor: Microsoft.AspNetCore.Http.Generated.GeneratedRouteBuilderExtensionsCore.MapGet1(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder, string, System.Delegate)
              at location: Microsoft.AspNetCore.Http.RequestDelegateGenerator{DirectorySeparator}Microsoft.AspNetCore.Http.RequestDelegateGenerator.RequestDelegateGenerator{DirectorySeparator}GeneratedRouteBuilderExtensions.g.cs:144:45
              intercepts 1 method invocation:
              - Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.MapGet(string, System.Delegate)
                - Program.cs:22:10
            """, null);
        Result.Verify(ExitCodes.Success);
    }
}
