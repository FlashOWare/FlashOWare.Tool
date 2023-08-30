using FlashOWare.Tool.Cli.Tests.CommandLine.IO;
using FlashOWare.Tool.Cli.Tests.Testing;

namespace FlashOWare.Tool.Cli.Tests.UsingDirectives;

public class UsingCounterTests : IntegrationTests
{
    [Fact]
    public async Task Count_ProjectUnderTest_FindAllOccurrences()
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
        string[] args = CreateArgs(project);
        //Act
        int exitCode = await RunAsync(args);
        //Assert
        Console.AssertOutput($"""
            Project: {Names.Project}
              System: 2
              System.Collections.Generic: 2
              System.Linq: 2
              System.Text: 2
              System.Threading.Tasks: 2
              System.Reflection: 1
            """);
        Assert.Equal(ExitCodes.Success, exitCode);
    }

    private static string[] CreateArgs(FileInfo project)
    {
        return new[] { "using", "count", project.FullName };
    }
}
