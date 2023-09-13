using FlashOWare.Tool.Cli.Tests.CommandLine.IO;
using FlashOWare.Tool.Cli.Tests.Testing;
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
        string[] args = CreateArgs(project.File);
        //Act
        await RunAsync(args);
        //Assert
        Console.VerifyOutput($"""
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

    private static string[] CreateArgs(FileInfo project)
    {
        return new[] { "using", "count", "--project", project.FullName };
    }
}
