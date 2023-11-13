using FlashOWare.Tool.Cli.IO;
using FlashOWare.Tool.Core.Interceptors;
using Microsoft.CodeAnalysis;

namespace FlashOWare.Tool.Cli;

public static partial class CliApplication
{
    private static void AddInterceptorCommand(RootCommand rootCommand, MSBuildWorkspace workspace, IFileSystemAccessor fileSystem)
    {
        var interceptorCommand = new Command("interceptor", "Discover experimental C# interceptors.");
        var listCommand = new Command("list", "Find and list experimental interceptors in a C# project.");

        listCommand.Add(Options.Project);
        listCommand.SetHandler(async (InvocationContext context) =>
        {
            FileInfo? project = context.ParseResult.GetValueForOption(Options.Project);
            project ??= fileSystem.GetSingleProject();

            await ListInterceptorAsync(workspace, project, context.Console, context.GetCancellationToken());
        });

        interceptorCommand.Add(listCommand);
        rootCommand.Add(interceptorCommand);
    }

    private static async Task ListInterceptorAsync(MSBuildWorkspace workspace, FileInfo projectFile, IConsole console, CancellationToken cancellationToken)
    {
        try
        {
            await Context.MSBuildMutex.WaitAsync(cancellationToken);
            Project project = await workspace.OpenProjectAsync(projectFile.FullName, null, cancellationToken);

            var result = await InterceptorLocator.ListAsync(project, cancellationToken);

            console.WriteLine("Warning: Interceptors are an experimental feature, currently available in preview mode.");
            console.WriteLine($"{nameof(Project)}: {result.ProjectName}");
        }
        catch (OperationCanceledException)
        {
            console.WriteLine("Operation canceled.");
        }
        finally
        {
            Context.MSBuildMutex.Release();
        }
    }
}
