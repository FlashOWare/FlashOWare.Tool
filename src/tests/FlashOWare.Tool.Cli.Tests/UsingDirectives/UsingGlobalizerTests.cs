using FlashOWare.Tool.Cli.Tests.IO;
using FlashOWare.Tool.Cli.Tests.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace FlashOWare.Tool.Cli.Tests.UsingDirectives;

//TODO: use System.CommandLine.IConsole
//TODO: restore old/original project after test run
//      via Workspace API
//      via Docker containerization
//      via FileSystem API
//TODO: Assert that ./GlobalUsings.cs contains exactly one single UsingDirectiveSyntax with Name "System" and with global Keyword
//TODO: Validate Input: <PROJECT>
//TODO: Redesign command-line arguments: flashoware using globalize <USING> [<PROJECT>] [options]

public class UsingGlobalizerTests : IDisposable
{
    private readonly TestTextWriter _output;

    public UsingGlobalizerTests()
    {
        _output = new TestTextWriter();
        Console.SetOut(_output);
    }

    void IDisposable.Dispose()
    {
        _output.Dispose();
    }

    [Fact]
    public async Task Globalize_MultipleDocuments_ReplacesWithGlobalUsingDirectives()
    {
        //Arrange
        //var msBuild = MSBuildLocator.RegisterDefaults();
        //using var originalWorkspace = MSBuildWorkspace.Create();
        //var originalProject = await originalWorkspace.OpenProjectAsync(FileSystemUtilities.TestProject);

        string[] args = CreateArgs(Usings.System, FileSystemUtilities.TestProject);
        //Act
        int exitCode = await CliApplication.RunAsync(args);
        //Assert
        Assert.Equal(ExitCodes.Success, exitCode);

        using var workspace = MSBuildWorkspace.Create();
        Project project = await workspace.OpenProjectAsync(FileSystemUtilities.TestProject);
        await AssertThatProjectHasNoTopLevelUsingDirectiveOfNameAsync(Usings.System, project);

        //bool changesApplied = originalWorkspace.TryApplyChanges(originalProject.Solution);
        //Assert.True(changesApplied, "Changes have not been applied.");

        _output.AssertText("""
            Project: ProjectUnderTest.NetCore(net6.0)
            6 occurrences of Using Directive "System" were globalized to "GlobalUsings.cs".
            """);
    }

    private static string[] CreateArgs(string localUsing, string project)
    {
        return new[] { "using", "globalize", localUsing, project };
    }

    private static async Task AssertThatProjectHasNoTopLevelUsingDirectiveOfNameAsync(string name, Project project)
    {
        foreach (Document document in project.Documents)
        {
            SyntaxNode? syntaxRoot = await document.GetSyntaxRootAsync();
            var compilationUnit = (CompilationUnitSyntax)syntaxRoot!;

            var usings = compilationUnit.Usings.Where(bool (UsingDirectiveSyntax usingNode) =>
            {
                return !usingNode.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword)
                    && usingNode.Name.ToString() == name;
            });
            Assert.Empty(usings);
        }
    }
}
