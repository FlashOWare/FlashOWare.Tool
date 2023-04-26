using FlashOWare.Tool.Core.UsingDirectives;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

ConsoleColor color = ForegroundColor;
ForegroundColor = ConsoleColor.Red;
WriteLine("2code ^ !2code...that is the question!");
ForegroundColor = color;

string path = Path.Combine("..", "..", "tests", "FlashOWare.Tool.ProjectUnderTest", "FlashOWare.Tool.ProjectUnderTest.csproj");
if (!File.Exists(path))
{
    path = Path.Combine("..", "..", "..", "..", "..", "tests", "FlashOWare.Tool.ProjectUnderTest", "FlashOWare.Tool.ProjectUnderTest.csproj");
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

var map = await UsingCounter.CountAsync(project);
foreach (var entry in map)
{
    WriteLine($"{entry.Key}: {entry.Value}");
}
