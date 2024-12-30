using FlashOWare.Tool.Cli.Tests.CommandLine.IO;
using FlashOWare.Tool.Cli.Tests.Testing;
using FlashOWare.Tool.Cli.Tests.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FlashOWare.Tool.Cli.Tests.UsingDirectives;

public class UsingCounterTests : IntegrationTests
{
    [Fact]
    public async Task Count_SdkStyleProject_FindAllOccurrences()
    {
        //Arrange
        var project = Workspace.CreateProject()
            .AddDocument("""
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;

                namespace ProjectUnderTest.NetCore
                {
                    internal class MyClass1
                    {
                    }
                }
                """, "MyClass1")
            .AddDocument("""
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;

                namespace ProjectUnderTest.NetCore;

                internal class MyClass2
                {
                }
                """, "MyClass2")
            .AddDocument("""
                using System.Reflection;

                [assembly: AssemblyDescription("A .NET tool that facilitates development workflows.")]
                [assembly: AssemblyCopyright("Copyright © FlashOWare 2023")]
                [assembly: AssemblyTrademark("")]
                [assembly: AssemblyCulture("")]
                """, Names.AssemblyInfo, Names.Properties)
            .AddDocument("""
                global using System.IO;
                global using System.Net.Http;
                global using System.Threading;
                """, Names.GlobalUsings, Names.Properties)
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10);
        string[] args = ["using", "count", "--project", project.File.FullName];
        //Act
        await RunAsync(args);
        //Assert
        Console.Verify($"""
            Project: {Names.Project}
              System: 2
              System.Collections.Generic: 2
              System.Linq: 2
              System.Text: 2
              System.Threading.Tasks: 2
              System.Reflection: 1
            """);
        Result.Verify(ExitCodes.Success);
    }

    [Fact]
    public async Task Count_SpecifyUsings_FindSpecifiedOccurrences()
    {
        //Arrange
        var project = Workspace.CreateProject()
            .AddDocument("""
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;

                namespace ProjectUnderTest.NetCore;

                internal class MyClass1
                {
                }
                """, "MyClass1")
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10);
        string[] args = ["using", "count", Usings.System, Usings.System_Linq, "--project", project.File.FullName];
        //Act
        await RunAsync(args);
        //Assert
        Console.Verify($"""
            Project: {Names.Project}
              System: 1
              System.Linq: 1
            """);
        Result.Verify(ExitCodes.Success);
    }

    [Fact]
    public async Task Count_ExplicitProjectFileDoesNotExist_FailsValidation()
    {
        //Arrange
        string project = "ProjectFileDoesNotExist.csproj";
        _ = Workspace.CreateProject()
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10);
        string[] args = ["using", "count", "--proj", project];
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyError($"File does not exist: '{project}'.");
        Result.Verify(ExitCodes.Error);
    }

    [Fact]
    public async Task Count_ExplicitSolutionFileDoesNotExist_FailsValidation()
    {
        //Arrange
        string solution = "SolutionFileDoesNotExist.sln";
        _ = await Workspace.CreateSolution()
            .InitializeAsync();
        string[] args = ["using", "count", "--sln", solution];
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyError($"File does not exist: '{solution}'.");
        Result.Verify(ExitCodes.Error);
    }

    [Fact]
    public async Task Count_VisualBasicProject_NotSupported()
    {
        //Arrange
        var project = Workspace.CreateProject(Language.VisualBasic)
            .AddDocument("""
                Public Class Class1

                End Class
                """, "Class1")
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60);
        string[] args = ["using", "count", "--proj", project.File.FullName];
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyContains(null, $"Cannot open project '{project.File}' because the language '{LanguageNames.VisualBasic}' is not supported.");
        Result.Verify(ExitCodes.Error);
    }

    [Fact]
    public async Task Count_ImplicitSingleProject_UseCurrentDirectory()
    {
        //Arrange
        _ = Workspace.CreateProject()
            .AddDocument("""
                using System;

                namespace ProjectUnderTest.NetCore;

                internal class MyClass1
                {
                }
                """, "MyClass1")
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10);
        string[] args = ["using", "count"];
        //Act
        await RunAsync(args);
        //Assert
        Console.Verify($"""
            Project: {Names.Project}
              System: 1
            """);
        Result.Verify(ExitCodes.Success);
    }

    [Fact]
    public async Task Count_ImplicitSingleSolution_UseCurrentDirectory()
    {
        //Arrange
        _ = await Workspace.CreateSolution()
            .AddCSharpProject(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10, static void (PhysicalProjectBuilder builder) => builder
                .AddDocument("""
                    using System;

                    namespace ProjectUnderTest.NetCore;

                    internal class MyClass1
                    {
                    }
                    """, "MyClass1"))
            .InitializeAsync();
        string[] args = ["using", "count"];
        //Act
        await RunAsync(args);
        //Assert
        Console.Verify($"""
            Solution: {Names.Solution}
              Project: {Names.Project}0
                System: 1
            """);
        Result.Verify(ExitCodes.Success);
    }

    [Fact]
    public async Task Count_ImplicitProjectOrSolutionMissing_Error()
    {
        //Arrange
        string[] args = ["using", "count"];
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyContains(null, "Specify a project or solution file. The current working directory does not contain a project or solution file.");
        Result.Verify(ExitCodes.Error);
    }

    [Fact]
    public async Task Count_ExplicitProjectAndSolution_Ambiguous()
    {
        //Arrange
        var project = Workspace.CreateProject()
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10);
        var solution = await Workspace.CreateSolution()
            .InitializeAsync();
        string[] args = ["using", "count", "--proj", project.FullName, "--sln", solution.FullName];
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyContains(null, "Both 'Project' and 'Solution' are specified. Specify either a 'Project' or a 'Solution', which are mutually exclusive.");
        Result.Verify(ExitCodes.Error);
    }

    [Fact]
    public async Task Count_ImplicitMultipleProjects_Ambiguous()
    {
        //Arrange
        _ = Workspace.CreateProject()
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10);
        _ = Workspace.CreateProject().WithProjectName("Ambiguous")
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10);
        string[] args = ["using", "count"];
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyContains(null, "Specify which project or solution file to use because this folder contains more than one project or solution file.");
        Result.Verify(ExitCodes.Error);
    }

    [Fact]
    public async Task Count_ImplicitMultipleSolutions_Ambiguous()
    {
        //Arrange
        _ = await Workspace.CreateSolution()
            .InitializeAsync();
        _ = await Workspace.CreateSolution().WithSolutionName("Ambiguous")
            .InitializeAsync();
        string[] args = ["using", "count"];
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyContains(null, "Specify which project or solution file to use because this folder contains more than one project or solution file.");
        Result.Verify(ExitCodes.Error);
    }

    [Fact]
    public async Task Count_ImplicitMultipleProjectsAndSolutions_Ambiguous()
    {
        //Arrange
        _ = Workspace.CreateProject()
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10);
        _ = await Workspace.CreateSolution()
            .InitializeAsync();
        string[] args = ["using", "count"];
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyContains(null, "Specify which project or solution file to use because this folder contains more than one project or solution file.");
        Result.Verify(ExitCodes.Error);
    }

    [Fact]
    public async Task Count_Solution_FindAllOccurrences()
    {
        //Arrange
        var solution = await Workspace.CreateSolution()
            .AddCSharpProject(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10, static void (PhysicalProjectBuilder builder) => builder
                .AddDocument("""
                    using System;
                    """)
                .AddDocument("""
                    using System;
                    using System.Collections.Generic;
                    """)
                .AddDocument("""
                    using System;
                    using System.Collections.Generic;
                    using System.Linq;
                    """)
                .AddDocument("""
                    using System;
                    using System.Collections.Generic;
                    using System.Linq;
                    using System.Text;
                    """)
                .AddDocument("""
                    using System;
                    using System.Collections.Generic;
                    using System.Linq;
                    using System.Text;
                    using System.Threading.Tasks;
                    """))
            .AddCSharpProject(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10, static void (PhysicalProjectBuilder builder) => builder
                .AddDocument("""
                    using System;
                    using System.Collections.Generic;
                    using System.IO;
                    using System.Linq;
                    using System.Net.Http;
                    using System.Threading;
                    using System.Threading.Tasks;
                    """))
            .InitializeAsync();
        string[] args = ["using", "count", "--solution", solution.File.FullName];
        //Act
        await RunAsync(args);
        //Assert
        Console.Verify($"""
            Solution: {Names.Solution}
              Project: {Names.Project}0
                System: 5
                System.Collections.Generic: 4
                System.Linq: 3
                System.Text: 2
                System.Threading.Tasks: 1
              Project: {Names.Project}1
                System: 1
                System.Collections.Generic: 1
                System.IO: 1
                System.Linq: 1
                System.Net.Http: 1
                System.Threading: 1
                System.Threading.Tasks: 1
            """);
        Result.Verify(ExitCodes.Success);
    }
}
