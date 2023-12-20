using FlashOWare.Tool.Cli.IO;
using FlashOWare.Tool.Core.Interceptors;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using System.CommandLine.IO;

namespace FlashOWare.Tool.Cli;

public static partial class CliApplication
{
    private static void AddInterceptorCommand(RootCommand rootCommand, MSBuildWorkspace workspace, IFileSystemAccessor fileSystem)
    {
        var interceptorCommand = new Command("interceptor", "Discover experimental C# interceptors.");
        var checkCommand = new Command("check", "Show whether experimental interceptors are supported or not by discovered .NET SDKs.");
        var listCommand = new Command("list", "Find and list experimental interceptors in a C# project.");

        checkCommand.SetHandler((InvocationContext context) =>
        {
            IEnumerable<VisualStudioInstance> instances = MSBuildLocator.QueryVisualStudioInstances();

            CheckInterceptors(instances, context.Console);
        });

        var groupOption = new Option<bool>(["--group-by-interceptors", "--group"], "Group the result by interceptors instead of the intercepted locations.");
        listCommand.Add(CliOptions.Project);
        listCommand.Add(groupOption);
        listCommand.SetHandler(async (InvocationContext context) =>
        {
            FileInfo? project = context.ParseResult.GetValueForOption(CliOptions.Project);
            project ??= fileSystem.GetSingleProject();

            bool groupByInterceptors = context.ParseResult.GetValueForOption(groupOption);

            await ListInterceptorsAsync(workspace, project, groupByInterceptors, context.Console, context.GetCancellationToken());
        });

        interceptorCommand.Add(checkCommand);
        interceptorCommand.Add(listCommand);
        rootCommand.Add(interceptorCommand);
    }

    private static void CheckInterceptors(IEnumerable<VisualStudioInstance> instances, IConsole console)
    {
        console.WriteLine(".NET SDKs:");

        IOrderedEnumerable<VisualStudioInstance> enumerable = instances
            .Where(static instance => (VisualStudioInstanceQueryOptions.Default.DiscoveryTypes & instance.DiscoveryType) == instance.DiscoveryType)
            .OrderBy(static instance => instance.Version);

        foreach (VisualStudioInstance instance in enumerable)
        {
            if (instance.Version.Major == 8)
            {
                console.WriteLine($"> {instance.Version} supports the 'interceptors' experimental feature by adding '<InterceptorsPreviewNamespaces>$(InterceptorsPreviewNamespaces);MyNamespace</InterceptorsPreviewNamespaces>' to your project.");
            }
            else if (instance.Version.Major == 7 && ((instance.Version.Minor == 0 && instance.Version.Build >= 400) || instance.Version.Minor > 0))
            {
                console.WriteLine($"> {instance.Version} supports the 'interceptors' experimental feature by adding '<Features>InterceptorsPreview</Features>' to your project.");
            }
            else
            {
                console.WriteLine($"> {instance.Version} does not support the 'interceptors' experimental feature.");
            }
        }

        console.WriteLine($"Current: {CliContext.MSBuild.Version}");
    }

    private static async Task ListInterceptorsAsync(MSBuildWorkspace workspace, FileInfo projectFile, bool groupByInterceptors, IConsole console, CancellationToken cancellationToken)
    {
        if (CliContext.MSBuild.Version is { Major: < 8 } and not { Major: 7, Minor: > 0 } and not { Major: 7, Minor: 0, Build: >= 400 })
        {
            console.Error.WriteLine($"The 'interceptors' experimental feature is not supported in .NET SDK {CliContext.MSBuild.Version}.");
            return;
        }

        try
        {
            await CliContext.MSBuildMutex.WaitAsync(cancellationToken);
            Project project = await workspace.OpenProjectAsync(projectFile.FullName, null, cancellationToken);

            var result = await InterceptorLocator.ListAsync(project, cancellationToken);

            console.WriteLine("Warning: Interceptors are an experimental feature, currently available in preview mode.");
            console.WriteLine($"{result}.");

            if (groupByInterceptors)
            {
                ListByInterceptor(result, project, console);
            }
            else
            {
                ListByIntercepted(result, project, console);
            }
        }
        catch (OperationCanceledException)
        {
            console.WriteLine("Operation canceled.");
        }
        finally
        {
            CliContext.MSBuildMutex.Release();
        }

        static void ListByIntercepted(InterceptorList result, Project project, IConsole console)
        {
            if (result.Interceptors.Count == 0)
            {
                return;
            }

            console.Write(Environment.NewLine);

            string? directory = Path.GetDirectoryName(project.FilePath);

            IEnumerable<IGrouping<string, (InterceptorInfo Interceptor, InterceptionInfo Interception)>> interceptions = result.Interceptors
                .SelectMany(static interceptor => interceptor.Interceptions, static (interceptor, interception) => (Interceptor: interceptor, Interception: interception))
                .GroupBy(static interception => interception.Interception.Attribute.FilePath);

            foreach (IGrouping<string, (InterceptorInfo Interceptor, InterceptionInfo Interception)> group in interceptions)
            {
                string filePath = directory is null ? group.Key : Path.GetRelativePath(directory, group.Key);

                if (group.TryGetNonEnumeratedCount(out int count))
                {
                    console.WriteLine($"{count} {(count == 1 ? "interception" : "interceptions")} in {filePath}");
                }
                else
                {
                    console.WriteLine($"Interceptions in {filePath}");
                }

                foreach ((InterceptorInfo Interceptor, InterceptionInfo Interception) interception in group)
                {
                    string document = directory is null ? interception.Interceptor.Document : Path.GetRelativePath(directory, interception.Interceptor.Document);
                    console.WriteLine($"- intercepted {interception.Interception.CallSite}");
                    console.WriteLine($"  at location {filePath}:{interception.Interception.Attribute.Line}:{interception.Interception.Attribute.Character}");
                    console.WriteLine($"  Interceptor {interception.Interceptor.Method}");
                    console.WriteLine($"  at location {document}:{interception.Interceptor.Line}:{interception.Interceptor.Character}");
                }
            }
        }

        static void ListByInterceptor(InterceptorList result, Project project, IConsole console)
        {
            if (result.Interceptors.Count == 0)
            {
                return;
            }

            console.Write(Environment.NewLine);

            if (result.Interceptors.Count == 1)
            {
                console.WriteLine($"1 Interceptor");
            }
            else
            {
                console.WriteLine($"{result.Interceptors.Count} Interceptors");
            }

            string? directory = Path.GetDirectoryName(project.FilePath);

            foreach (InterceptorInfo interceptor in result.Interceptors)
            {
                string document = directory is null ? interceptor.Document : Path.GetRelativePath(directory, interceptor.Document);
                console.WriteLine($"- Interceptor: {interceptor.Method}");
                console.WriteLine($"  at location: {document}:{interceptor.Line}:{interceptor.Character}");
                console.WriteLine($"  intercepts {interceptor.Interceptions.Count} method {(interceptor.Interceptions.Count == 1 ? "invocation" : "invocations")}:");

                foreach (IGrouping<string, InterceptionInfo> interceptions in interceptor.Interceptions.GroupBy(static interception => interception.CallSite.Method))
                {
                    console.WriteLine($"  - {interceptions.Key}");
                    foreach (InterceptionInfo interception in interceptions)
                    {
                        string filePath = directory is null ? interception.Attribute.FilePath : Path.GetRelativePath(directory, interception.Attribute.FilePath);
                        console.WriteLine($"    - {filePath}:{interception.Attribute.Line}:{interception.Attribute.Character}");
                    }
                }
            }
        }
    }
}
