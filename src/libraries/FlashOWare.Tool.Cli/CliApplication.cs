using FlashOWare.Tool.Cli.CodeAnalysis;
using FlashOWare.Tool.Cli.Diagnostics;
using FlashOWare.Tool.Core.UsingDirectives;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FlashOWare.Tool.Cli;

public static class CliApplication
{
    public static async Task<int> RunAsync(string[] args)
    {
        //TODO: how to deal with Trivia when removing UsingDirectiveSyntax nodes?
        //TODO: do not modify SDK-style project files!
        //TODO: when the UsingGlobalizer's MVP is complete, Stefan must dance!
        //TODO: use System.CommandLine!

        ConsoleColor color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("2code ^ !2code...that is the question!");
        Console.ForegroundColor = color;

        using var cancellation = new CancellationTokenSource();
        Action<PosixSignalContext> handler = (PosixSignalContext context) =>
        {
            Debug.Assert(context.Signal == PosixSignal.SIGINT || context.Signal == PosixSignal.SIGQUIT || context.Signal == PosixSignal.SIGTERM);

            context.Cancel = true;
            cancellation.Cancel();
        };
        using var sigInt = PosixSignalRegistration.Create(PosixSignal.SIGINT, handler);
        using var sigQuit = PosixSignalRegistration.Create(PosixSignal.SIGQUIT, handler);
        using var sigTerm = PosixSignalRegistration.Create(PosixSignal.SIGTERM, handler);

        var msBuild = MSBuildLocator.RegisterDefaults();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"MSBuild ({msBuild.DiscoveryType}): {msBuild.Name} {msBuild.Version}");
        Console.ForegroundColor = color;

        var properties = ImmutableDictionary<string, string>.Empty.Add("Configuration", "Release");
        using var workspace = MSBuildWorkspace.Create(properties);
        workspace.WorkspaceFailed += OnWorkspaceFailed;

        if (args is ["using", "count", string path1])
        {
            await CountUsingsAsync(workspace, path1, cancellation.Token);
        }
        else if (args is ["using", "globalize", string localUsing, string path2])
        {
            await GlobalizeUsingsAsync(workspace, path2, localUsing, cancellation.Token);
        }
        else
        {
            Console.WriteLine("""
                Commands:
                  flashoware using count <PROJECT>
                  flashoware using globalize <USING> <PROJECT>
                """);
        }

        workspace.WorkspaceFailed -= OnWorkspaceFailed;
        return ExitCodes.Success;
    }

    private static void OnWorkspaceFailed(object? sender, WorkspaceDiagnosticEventArgs e)
    {
        Console.WriteLine(e.Diagnostic.ToString());
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
