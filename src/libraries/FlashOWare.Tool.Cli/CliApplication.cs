using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace FlashOWare.Tool.Cli;

public static partial class CliApplication
{
    public static async Task<int> RunAsync(string[] args)
    {
        var msBuild = MSBuildLocator.RegisterDefaults();

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
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("2code ^ !2code...that is the question!");
                Console.ForegroundColor = color;
            }

            if (context.BindingContext.ParseResult.GetValueForOption(infoOption))
            {
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"MSBuild ({msBuild.DiscoveryType}): {msBuild.Name} {msBuild.Version}");
                Console.ForegroundColor = color;
            }
        });

        AddUsingCommand(rootCommand, workspace);

        int exitCode = await rootCommand.InvokeAsync(args);
        workspace.WorkspaceFailed -= OnWorkspaceFailed;
        return exitCode;
    }

    private static void OnWorkspaceFailed(object? sender, WorkspaceDiagnosticEventArgs e)
    {
        Console.WriteLine(e.Diagnostic.ToString());
    }
}
