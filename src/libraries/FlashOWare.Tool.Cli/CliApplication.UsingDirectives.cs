using FlashOWare.Tool.Cli.CodeAnalysis;
using FlashOWare.Tool.Core.UsingDirectives;
using Microsoft.CodeAnalysis;

namespace FlashOWare.Tool.Cli;

public static partial class CliApplication
{
    private static void AddUsingCommand(RootCommand rootCommand, MSBuildWorkspace workspace)
    {
        var usingCommand = new Command("using", " Analyze or refactor C# using directives.");
        var countCommand = new Command("count", "Count and list all top-level using directives of a C# project.");
        var globalizeCommand = new Command("globalize", "Move a top-level using directive to a global using directive in a C# project.");

        var projectArgument = new Argument<FileInfo>("PROJECT", "The project file to operate on.");

        countCommand.Add(projectArgument);
        countCommand.SetHandler(async (InvocationContext context) =>
        {
            FileInfo project = context.ParseResult.GetValueForArgument(projectArgument);

            await CountUsingsAsync(workspace, project.FullName, context.GetCancellationToken());
        });

        var usingArgument = new Argument<string>("USING", "The name of the top-level using directive to convert to a global using directive.");
        globalizeCommand.Add(usingArgument);
        globalizeCommand.Add(projectArgument);
        globalizeCommand.SetHandler(async (InvocationContext context) =>
        {
            string localUsing = context.ParseResult.GetValueForArgument(usingArgument);
            FileInfo project = context.ParseResult.GetValueForArgument(projectArgument);

            await GlobalizeUsingsAsync(workspace, project.FullName, localUsing, context.GetCancellationToken());
        });

        usingCommand.Add(countCommand);
        usingCommand.Add(globalizeCommand);
        rootCommand.Add(usingCommand);
    }

    private static async Task CountUsingsAsync(MSBuildWorkspace workspace, string projectFilePath, CancellationToken cancellationToken)
    {
        try
        {
            Project project = await workspace.OpenProjectAsync(projectFilePath, null, cancellationToken);

            var result = await UsingCounter.CountAsync(project, cancellationToken);
            Console.WriteLine($"{nameof(Project)}: {result.ProjectName}");
            foreach (var usingDirective in result.Usings)
            {
                Console.WriteLine($"  {usingDirective.Name}: {usingDirective.Occurrences}");
            }
        }
        catch (OperationCanceledException)
        {
            await Console.Out.WriteLineAsync("Operation canceled.");
        }
    }

    private static async Task GlobalizeUsingsAsync(MSBuildWorkspace workspace, string projectFilePath, string localUsing, CancellationToken cancellationToken)
    {
        try
        {
            Project project = await workspace.OpenProjectAsync(projectFilePath, null, cancellationToken);

            workspace.ThrowIfCannotApplyChanges(ApplyChangesKind.AddDocument, ApplyChangesKind.ChangeDocument);
            var result = await UsingGlobalizer.GlobalizeAsync(project, localUsing, cancellationToken);
            workspace.ApplyChanges(result.Project.Solution);

            Console.WriteLine($"{nameof(Project)}: {result.Project.Name}");

            if (result.Using.Occurrences == 0)
            {
                Console.WriteLine($"""No occurrences of Using Directive "{localUsing}" were globalized.""");
            }
            else if (result.Using.Occurrences == 1)
            {
                Console.WriteLine($"""1 occurrence of Using Directive "{localUsing}" was globalized to "{result.TargetDocument}".""");
            }
            else
            {
                Console.WriteLine($"""{result.Using.Occurrences} occurrences of Using Directive "{localUsing}" were globalized to "{result.TargetDocument}".""");
            }
        }
        catch (OperationCanceledException)
        {
            await Console.Out.WriteLineAsync("Operation canceled.");
        }
    }
}
