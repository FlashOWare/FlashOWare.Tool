using FlashOWare.Tool.Cli.Tests.CommandLine.IO;
using FlashOWare.Tool.Cli.Tests.IO;
using FlashOWare.Tool.Cli.Tests.Testing;

namespace FlashOWare.Tool.Cli.Tests.UsingDirectives;

//TODO: Validate Input: [<PROJECT>]
//TODO: Redesign command-line arguments: flashoware using globalize <USING> [<PROJECT>] [options]

public class UsingGlobalizerTests : IntegrationTests
{
    [Fact]
    public async Task Globalize_MultipleDocuments_ReplacesWithGlobalUsingDirectives()
    {
        //Arrange
        FileInfo project = Workspace.CSharp.CreateProject(Texts.CreateCSharpLibraryProject(), Names.CSharpProject);
        Workspace.CSharp.CreateDocument("""
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
            """, "MyClass1");
        Workspace.CSharp.CreateDocument("""
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;

            namespace ProjectUnderTest.NetCore;

            internal class MyClass2
            {
            }
            """, "MyClass2");
        Workspace.CSharp.CreateDocument("""
            using System.Reflection;

            [assembly: AssemblyDescription("A .NET tool that facilitates development workflows.")]
            [assembly: AssemblyCopyright("Copyright © FlashOWare 2023")]
            [assembly: AssemblyTrademark("")]
            [assembly: AssemblyCulture("")]
            """, "AssemblyInfo.cs", Names.Properties);
        Workspace.CSharp.CreateDocument("""
            global using System.IO;
            global using System.Net.Http;
            global using System.Threading;
            """, "GlobalUsings.cs", Names.Properties);
        string[] args = CreateArgs(Usings.System, project);
        //Act
        int exitCode = await CliApplication.RunAsync(args, Console);
        //Assert
        Console.AssertOutput($"""
            Project: {Names.Project}
            2 occurrences of Using Directive "System" were globalized to "GlobalUsings.cs".
            """);
        Workspace.AssertFiles(PhysicalDocumentList.Create("""
            global using System;

            """, "GlobalUsings.cs")
            .Add("""
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
            .Add("""
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;

            namespace ProjectUnderTest.NetCore;

            internal class MyClass2
            {
            }
            """, "MyClass2.cs")
            .Add(Texts.CreateCSharpLibraryProject(), Names.CSharpProject)
            .Add("""
            using System.Reflection;

            [assembly: AssemblyDescription("A .NET tool that facilitates development workflows.")]
            [assembly: AssemblyCopyright("Copyright © FlashOWare 2023")]
            [assembly: AssemblyTrademark("")]
            [assembly: AssemblyCulture("")]
            """, "AssemblyInfo.cs", Names.Properties)
            .Add("""
            global using System.IO;
            global using System.Net.Http;
            global using System.Threading;
            """, "GlobalUsings.cs", Names.Properties));
        Assert.Equal(ExitCodes.Success, exitCode);
    }

    private static string[] CreateArgs(string localUsing, FileInfo project)
    {
        return new[] { "using", "globalize", localUsing, project.FullName };
    }
}
