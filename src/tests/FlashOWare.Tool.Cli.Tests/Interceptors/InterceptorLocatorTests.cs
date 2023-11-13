using FlashOWare.Tool.Cli.Tests.CommandLine.IO;
using FlashOWare.Tool.Cli.Tests.Testing;
using Microsoft.CodeAnalysis.CSharp;

namespace FlashOWare.Tool.Cli.Tests.Interceptors;

public class InterceptorLocatorTests : IntegrationTests
{
    //TODO: Use an actual NuGet package that contains an Interceptor-Generator

    //TODO: Test against Native-AOT Web-API
    //if that does not work, we may need to compile the project with the [Generator] and see if the auto-generated documents are actually in the Project

    //TODO
    // display which methods are intercepted
    // display by which method these are intercepted

    //TODO: Display
    // Document (with interceptions)
    // C:\...\MyClass1.cs
    // - MyClass.MyMethod1, 5, 16
    //   - C:\...\Generated.g.cs, MyNamespace.GeneratedType.MyInterceptorMethod(double)
    // - MyClass.MyMethod2, 6, 16
    // ...

    [Theory(Skip = "TODO")]
    [InlineData(null)]
    [InlineData("--proj")]
    [InlineData("--project")]
    public async Task List_WithGenerator_ListsInterceptors(string? option)
    {
        //Arrange
        var project = Workspace.CreateProject()
            .AddDocument("", "")
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net70, LanguageVersion.CSharp11);
        //Act
        await (option is null ? RunAsync("interceptor", "list") : RunAsync("interceptor", "list", option, project.File.FullName));
        //Assert
        Console.Verify($"""
            TODO
            """);
        Result.Verify(ExitCodes.Success);
    }
}
