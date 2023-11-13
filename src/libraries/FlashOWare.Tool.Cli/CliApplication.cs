using FlashOWare.Tool.Cli.CommandLine;
using FlashOWare.Tool.Cli.IO;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace FlashOWare.Tool.Cli;

public static partial class CliApplication
{
    public static async Task<int> RunAsync(string[] args, IConsole? console = null, VisualStudioInstance? msBuild = null, IFileSystemAccessor? fileSystem = null)
    {
        msBuild ??= MSBuildLocator.RegisterDefaults();
        fileSystem ??= FileSystemAccessor.System;

        var properties = ImmutableDictionary<string, string>.Empty.Add("Configuration", "Release");
        using var workspace = MSBuildWorkspace.Create(properties);
        workspace.WorkspaceFailed += OnWorkspaceFailed;

        var aboutOption = new Option<bool>(new string[] { "--about", "-a", "-!" }, "Show application information");
        var infoOption = new Option<bool>(new string[] { "--info", "-i", "-#" }, "Show environment information");

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
                context.Console.WriteLine(ConsoleColor.Green, $"MSBuild ({msBuild.DiscoveryType}): {msBuild.Name} {msBuild.Version}");
            }
        });

        AddInterceptorCommand(rootCommand, workspace, fileSystem);
        AddUsingCommand(rootCommand, workspace, fileSystem);

        int exitCode = await rootCommand.InvokeAsync(args, console);
        workspace.WorkspaceFailed -= OnWorkspaceFailed;
        return exitCode;
    }

    private static void OnWorkspaceFailed(object? sender, WorkspaceDiagnosticEventArgs e)
    {
        Console.Error.WriteLine(e.Diagnostic.ToString());
    }
}
