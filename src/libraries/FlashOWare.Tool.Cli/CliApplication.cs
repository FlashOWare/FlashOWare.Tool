using FlashOWare.Tool.Core.UsingDirectives;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace FlashOWare.Tool.Cli;

public static class CliApplication
{
    public static async Task<int> RunAsync(string[] args)
    {
        ConsoleColor color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("2code ^ !2code...that is the question!");
        Console.ForegroundColor = color;

        string path = Path.Combine("..", "..", "tests", "ProjectUnderTest.NetCore", "ProjectUnderTest.NetCore.csproj");
        if (!File.Exists(path))
        {
            path = Path.Combine("..", "..", "..", "..", "..", "tests", "ProjectUnderTest.NetCore", "ProjectUnderTest.NetCore.csproj");
        }

        //TODO
        //MSBuildLocator.RegisterDefaults();

        //string msBuild = @"C:\Program Files\dotnet\sdk\7.0.202";
        string msBuild = @"C:\Program Files\dotnet\sdk\7.0.203";
        MSBuildLocator.RegisterMSBuildPath(msBuild);

        //TODO: dispose IDisposable
        var workspace = MSBuildWorkspace.Create();
        //workspace.LoadMetadataForReferencedProjects = true;
        //Workspace.WorkspaceFailed
        //OpenSolutionAsync
        Project project = await workspace.OpenProjectAsync(path);
        //project.Solution (immutable)
        //workspace.CurrentSolution (mutable)

        //var newSolution = project.Solution.WithDocumentSyntaxRoot();
        //project.AddDocument(); //only adds to memory, not written to disk, use System.IO.File
        //workspace.TryApplyChanges(); //does not change files on disk, write via System.IO.File

        var result = await UsingCounter.CountAsync(project);
        foreach (var usingDirective in result.Usings)
        {
            Console.WriteLine($"{usingDirective.Name}: {usingDirective.Occurrences}");
        }

        return 0;
    }
}
