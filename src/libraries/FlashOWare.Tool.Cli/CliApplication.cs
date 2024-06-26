using FlashOWare.Tool.Cli.CommandLine;
using FlashOWare.Tool.Cli.IO;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.CommandLine.IO;

namespace FlashOWare.Tool.Cli;

public static partial class CliApplication
{
    public static Task<int> RunAsync(string[] args)
    {
        VisualStudioInstance msBuild = MSBuildLocator.RegisterDefaults();
        IConsole console = new SystemConsole();
        IFileSystemAccessor fileSystem = FileSystemAccessor.System;

        CliContext.InitializeApp(msBuild);
        return RunAsync(args, console, fileSystem);
    }

    public static Task<int> RunAsync(string[] args, IConsole console, VisualStudioInstance msBuild, IFileSystemAccessor fileSystem)
    {
        CliContext.InitializeTest(msBuild);
        return RunAsync(args, console, fileSystem);
    }

    private static async Task<int> RunAsync(string[] args, IConsole console, IFileSystemAccessor fileSystem)
    {
        var properties = ImmutableDictionary<string, string>.Empty.Add("Configuration", "Release");
        using var workspace = MSBuildWorkspace.Create(properties);
        workspace.WorkspaceFailed += OnWorkspaceFailed;

        var aboutOption = new Option<bool>(["--about", "-a", "-!"], "Show application information");
        var infoOption = new Option<bool>(["--info", "-i", "-#"], "Show environment information");

        var rootCommand = new RootCommand("A .NET tool that facilitates development workflows.")
        {
            Name = "flashoware",
        };
        rootCommand.Add(aboutOption);
        rootCommand.Add(infoOption);

        rootCommand.SetHandler((InvocationContext context) =>
        {
            if (context.BindingContext.ParseResult.GetValueForOption(aboutOption))
            {
                context.Console.WriteLine(ConsoleColor.Red, "2code ^ !2code...that is the question!");
            }

            if (context.BindingContext.ParseResult.GetValueForOption(infoOption))
            {
                context.Console.WriteLine(ConsoleColor.Green, $"MSBuild ({CliContext.MSBuild.DiscoveryType}): {CliContext.MSBuild.Name} {CliContext.MSBuild.Version}");
            }
        });

        AddInterceptorCommand(rootCommand, workspace, fileSystem);
        AddUsingCommand(rootCommand, workspace, fileSystem);

        int exitCode = await rootCommand.InvokeAsync(args, console);
        workspace.WorkspaceFailed -= OnWorkspaceFailed;
        CliContext.Dispose();
        return exitCode;

        void OnWorkspaceFailed(object? sender, WorkspaceDiagnosticEventArgs e)
        {
            console.WriteErrorLine(ConsoleColor.Yellow, e.Diagnostic.ToString());
        }
    }
}
