using FlashOWare.Tool.Cli.CodeAnalysis;
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
        Project project = await workspace.OpenProjectAsync(path, null, CancellationToken.None);

        var result = await UsingCounter.CountAsync(project);
        Console.WriteLine($"{nameof(Project)}: {result.ProjectName}");
        foreach (var usingDirective in result.Usings)
        {
            Console.WriteLine($"{usingDirective.Name}: {usingDirective.Occurrences}");
        }

        workspace.ThrowIfCannotApplyChanges(ApplyChangesKind.AddDocument, ApplyChangesKind.ChangeDocument);
        var newProject = await UsingGlobalizer.GlobalizeAsync(project, "System");
        workspace.ApplyChanges(newProject.Solution);

        return 0;
    }
}
