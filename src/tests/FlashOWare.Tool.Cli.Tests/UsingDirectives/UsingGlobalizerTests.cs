using FlashOWare.Tool.Cli.Tests.CommandLine.IO;
using FlashOWare.Tool.Cli.Tests.Testing;
using FlashOWare.Tool.Cli.Tests.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FlashOWare.Tool.Cli.Tests.UsingDirectives;

public class UsingGlobalizerTests : IntegrationTests
{
    [Fact]
    public async Task Globalize_SdkStyleProject_ReplacesWithGlobalUsingDirectives()
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
        string[] args = new[] { "using", "globalize", Usings.System, "--project", project.File.FullName };
        //Act
        await RunAsync(args);
        //Assert
        Console.Verify($"""
            Project: {Names.Project}
            2 occurrences of Using Directive "{Usings.System}" were globalized to "GlobalUsings.cs".
            """);
        Workspace.CreateExpectation()
            .AppendFile("""
                global using System;

                """, Names.GlobalUsings)
            .AppendFile("""
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
                """, "MyClass1.cs")
            .AppendFile("""
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;

                namespace ProjectUnderTest.NetCore;

                internal class MyClass2
                {
                }
                """, "MyClass2.cs")
            .AppendFile(ProjectText.Create(TargetFramework.Net60, LanguageVersion.CSharp10), Names.CSharpProject)
            .AppendFile("""
                using System.Reflection;

                [assembly: AssemblyDescription("A .NET tool that facilitates development workflows.")]
                [assembly: AssemblyCopyright("Copyright © FlashOWare 2023")]
                [assembly: AssemblyTrademark("")]
                [assembly: AssemblyCulture("")]
                """, Names.AssemblyInfo, Names.Properties)
            .AppendFile("""
                global using System.IO;
                global using System.Net.Http;
                global using System.Threading;
                """, Names.GlobalUsings, Names.Properties)
            .Verify();
        Result.Verify(ExitCodes.Success);
    }

    [Fact]
    public async Task Globalize_DotNetFrameworkProject_ReplacesWithGlobalUsingDirectives()
    {
        //Arrange
        var project = Workspace.CreateProject()
            .AddDocument("""
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;

                namespace ProjectUnderTest.NetFx
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

                namespace ProjectUnderTest.NetFx;

                internal class MyClass2
                {
                }
                """, "MyClass2")
            .AddDocument($"""
                using System.Reflection;
                using System.Runtime.InteropServices;

                [assembly: AssemblyTitle("ProjectUnderTest.NetFx")]
                [assembly: AssemblyDescription("A .NET tool that facilitates development workflows.")]
                [assembly: AssemblyConfiguration("")]
                [assembly: AssemblyCompany("")]
                [assembly: AssemblyProduct("ProjectUnderTest.NetFx")]
                [assembly: AssemblyCopyright("Copyright © FlashOWare 2023")]
                [assembly: AssemblyTrademark("")]
                [assembly: AssemblyCulture("")]

                [assembly: ComVisible(false)]

                [assembly: Guid("{ProjectOptions.Default.AssemblyGuid}")]

                [assembly: AssemblyVersion("1.0.0.0")]
                [assembly: AssemblyFileVersion("1.0.0.0")]
                """, Names.AssemblyInfo, Names.Properties)
            .AddDocument("""
                global using System.IO;
                global using System.Net.Http;
                global using System.Threading;
                """, Names.GlobalUsings, Names.Properties)
            .Initialize(ProjectKind.Classic, TargetFramework.Net472, LanguageVersion.CSharp10);
        string[] args = new[] { "using", "globalize", Usings.System, "--project", project.File.FullName };
        //Act
        await RunAsync(args);
        //Assert
        Console.Verify($"""
            Project: {Names.Project}
            2 occurrences of Using Directive "{Usings.System}" were globalized to "GlobalUsings.cs".
            """);
        string[] files = { Names.GlobalUsings, "MyClass1.cs", "MyClass2.cs", Path.Combine(Names.Properties, Names.AssemblyInfo), Path.Combine(Names.Properties, Names.GlobalUsings) };
        Workspace.CreateExpectation()
            .AppendFile("""
                global using System;

                """, Names.GlobalUsings)
            .AppendFile("""
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;

                namespace ProjectUnderTest.NetFx
                {
                    internal class MyClass1
                    {
                    }
                }
                """, "MyClass1.cs")
            .AppendFile("""
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;

                namespace ProjectUnderTest.NetFx;

                internal class MyClass2
                {
                }
                """, "MyClass2.cs")
            .AppendFile(ProjectText.CreateNonSdk(TargetFramework.Net472, LanguageVersion.CSharp10, files), Names.CSharpProject)
            .AppendFile($"""
                using System.Reflection;
                using System.Runtime.InteropServices;

                [assembly: AssemblyTitle("ProjectUnderTest.NetFx")]
                [assembly: AssemblyDescription("A .NET tool that facilitates development workflows.")]
                [assembly: AssemblyConfiguration("")]
                [assembly: AssemblyCompany("")]
                [assembly: AssemblyProduct("ProjectUnderTest.NetFx")]
                [assembly: AssemblyCopyright("Copyright © FlashOWare 2023")]
                [assembly: AssemblyTrademark("")]
                [assembly: AssemblyCulture("")]

                [assembly: ComVisible(false)]

                [assembly: Guid("{ProjectOptions.Default.AssemblyGuid}")]

                [assembly: AssemblyVersion("1.0.0.0")]
                [assembly: AssemblyFileVersion("1.0.0.0")]
                """, Names.AssemblyInfo, Names.Properties)
            .AppendFile("""
                global using System.IO;
                global using System.Net.Http;
                global using System.Threading;
                """, Names.GlobalUsings, Names.Properties)
            .Verify();
        Result.Verify(ExitCodes.Success);
    }

    [Fact]
    public async Task Globalize_NoSpecificUsingsWithForce_ReplacesAllUsingDirectives()
    {
        //Arrange
        var project = Workspace.CreateProject()
            .AddDocument("""
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Linq;
                using System.Net.Http;
                using System.Text;
                using System.Threading;
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
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10);
        string[] args = new[] { "using", "globalize", "--project", project.File.FullName, "--force" };
        //Act
        await RunAsync(args);
        //Assert
        Console.Verify($"""
            Project: {Names.Project}
            13 occurrences of 8 Using Directives were globalized to "GlobalUsings.cs".
            """);
        Workspace.CreateExpectation()
            .AppendFile("""
                global using System;
                global using System.Collections.Generic;
                global using System.IO;
                global using System.Linq;
                global using System.Net.Http;
                global using System.Text;
                global using System.Threading;
                global using System.Threading.Tasks;

                """, Names.GlobalUsings)
            .AppendFile("""

                namespace ProjectUnderTest.NetCore
                {
                    internal class MyClass1
                    {
                    }
                }
                """, "MyClass1.cs")
            .AppendFile("""

                namespace ProjectUnderTest.NetCore;

                internal class MyClass2
                {
                }
                """, "MyClass2.cs")
            .AppendFile(ProjectText.Create(TargetFramework.Net60, LanguageVersion.CSharp10), Names.CSharpProject)
            .Verify();
        Result.Verify(ExitCodes.Success);
    }

    [Fact]
    public async Task Globalize_NoSpecificUsingsWithoutForce_ForceIsRequired()
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
        string[] args = new[] { "using", "globalize", "--project", project.File.FullName };
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyContains(null, "No usings specified. To globalize all top-level using directives, run the command with '--force' option.");
        Result.Verify(ExitCodes.Error);
    }

    [Fact]
    public async Task Globalize_ManySpecificUsings_ReplacesSpecifiedUsingDirectives()
    {
        //Arrange
        var project = Workspace.CreateProject()
            .AddDocument("""
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Linq;
                using System.Net.Http;
                using System.Text;
                using System.Threading;
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
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10);
        string[] args = new[] { "using", "globalize", Usings.System_Collections_Generic, Usings.System_Linq, "--project", project.File.FullName };
        //Act
        await RunAsync(args);
        //Assert
        Console.Verify($"""
            Project: {Names.Project}
            4 occurrences of 2 Using Directives were globalized to "GlobalUsings.cs".
            """);
        Workspace.CreateExpectation()
            .AppendFile("""
                global using System.Collections.Generic;
                global using System.Linq;

                """, Names.GlobalUsings)
            .AppendFile("""
                using System;
                using System.IO;
                using System.Net.Http;
                using System.Text;
                using System.Threading;
                using System.Threading.Tasks;

                namespace ProjectUnderTest.NetCore
                {
                    internal class MyClass1
                    {
                    }
                }
                """, "MyClass1.cs")
            .AppendFile("""
                using System;
                using System.Text;
                using System.Threading.Tasks;

                namespace ProjectUnderTest.NetCore;

                internal class MyClass2
                {
                }
                """, "MyClass2.cs")
            .AppendFile(ProjectText.Create(TargetFramework.Net60, LanguageVersion.CSharp10), Names.CSharpProject)
            .Verify();
        Result.Verify(ExitCodes.Success);
    }

    [Fact]
    public async Task Globalize_ExplicitProjectFileDoesNotExist_FailsValidation()
    {
        //Arrange
        string project = "ProjectFileDoesNotExist.csproj";
        _ = Workspace.CreateProject()
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10);
        string[] args = new[] { "using", "globalize", Usings.System, "--proj", project };
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyError($"File does not exist: '{project}'.");
        Result.Verify(ExitCodes.Error);
    }

    [Fact]
    public async Task Globalize_VisualBasicProject_NotSupported()
    {
        //Arrange
        var project = Workspace.CreateProject(Language.VisualBasic)
            .AddDocument("""
                Public Class Class1

                End Class
                """, "Class1")
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60);
        string[] args = new[] { "using", "globalize", Usings.System, "--proj", project.File.FullName };
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyContains(null, $"Cannot open project '{project.File}' because the language '{LanguageNames.VisualBasic}' is not supported.");
        Result.Verify(ExitCodes.Error);
    }

    [Fact]
    public async Task Globalize_ImplicitSingleProject_UseCurrentDirectory()
    {
        //Arrange
        _ = Workspace.CreateProject()
            .AddDocument("""
                using System;
                using System.Collections.Generic;

                namespace ProjectUnderTest.NetCore;

                internal class MyClass1
                {
                }
                """, "MyClass1")
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10);
        string[] args = new[] { "using", "globalize", Usings.System };
        //Act
        await RunAsync(args);
        //Assert
        Console.Verify($"""
            Project: {Names.Project}
            1 occurrence of Using Directive "{Usings.System}" was globalized to "GlobalUsings.cs".
            """);
        Workspace.CreateExpectation()
            .AppendFile("""
                global using System;

                """, Names.GlobalUsings)
            .AppendFile("""
                using System.Collections.Generic;

                namespace ProjectUnderTest.NetCore;

                internal class MyClass1
                {
                }
                """, "MyClass1.cs")
            .AppendFile(ProjectText.Create(TargetFramework.Net60, LanguageVersion.CSharp10), Names.CSharpProject)
            .Verify();
        Result.Verify(ExitCodes.Success);
    }

    [Fact]
    public async Task Globalize_ImplicitProjectMissing_Error()
    {
        //Arrange
        string[] args = new[] { "using", "globalize", Usings.System };
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyContains(null, "Specify a project file. The current working directory does not contain a project file.");
        Result.Verify(ExitCodes.Error);
    }

    [Fact]
    public async Task Globalize_ImplicitMultipleProjects_Ambiguous()
    {
        //Arrange
        _ = Workspace.CreateProject()
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10);
        _ = Workspace.CreateProject().WithProjectName("Ambiguous")
            .Initialize(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10);
        string[] args = new[] { "using", "globalize", Usings.System };
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyContains(null, "Specify which project file to use because this folder contains more than one project file.");
        Result.Verify(ExitCodes.Error);
    }
}
