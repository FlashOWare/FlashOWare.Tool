using FlashOWare.Tool.Cli.Tests.CommandLine.IO;
using FlashOWare.Tool.Cli.Tests.Testing;
using FlashOWare.Tool.Cli.Tests.Workspaces;
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
        string[] args = CreateArgs(Usings.System, project.File);
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyOutput($"""
            Project: {Names.Project}
            2 occurrences of Using Directive "System" were globalized to "GlobalUsings.cs".
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
            .AppendFile(Projects.CreateProject(ProjectKind.SdkStyle, TargetFramework.Net60, LanguageVersion.CSharp10), Names.CSharpProject)
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
        string[] args = CreateArgs(Usings.System, project.File);
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyOutput($"""
            Project: {Names.Project}
            2 occurrences of Using Directive "System" were globalized to "GlobalUsings.cs".
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
            .AppendFile(Projects.CreateProject(ProjectKind.Classic, TargetFramework.Net472, LanguageVersion.CSharp10, files), Names.CSharpProject)
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

    private static string[] CreateArgs(string localUsing, FileInfo project)
    {
        return new[] { "using", "globalize", localUsing, project.FullName };
    }
}
