using FlashOWare.Tool.Cli.Tests.Diagnostics;
using FlashOWare.Tool.Cli.Tests.Extensions;
using FlashOWare.Tool.Cli.Tests.Hosting;
using System.ComponentModel;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Sdk;

public partial class DotNet
{
    internal Task<string> NewAsync(DotNetNewTemplate template)
    {
        return NewAsync(template, DotNetCliOptions.None);
    }

    internal Task<string> NewAsync(DotNetNewTemplate template, string name)
    {
        DotNetCliOptions options = new() { Name = name };
        return NewAsync(template, options);
    }

    internal Task<string> NewAsync(DotNetNewTemplate template, bool noRestore)
    {
        DotNetCliOptions options = new() { NoRestore = noRestore };
        return NewAsync(template, options);
    }

    internal Task<string> NewAsync(DotNetNewTemplate template, string name, bool noRestore)
    {
        DotNetCliOptions options = new() { Name = name, NoRestore = noRestore };
        return NewAsync(template, options);
    }

    private Task<string> NewAsync(DotNetNewTemplate template, DotNetCliOptions options)
    {
        return template switch
        {
            DotNetNewTemplate.Unspecified => throw new InvalidOperationException("Template not specified."),
            DotNetNewTemplate.AspNetCoreWebApiNativeAot => NewAspNetCoreWebApiNativeAotAsync(options),
            DotNetNewTemplate.SolutionFile => NewSolutionAsync(options),
            _ => throw new InvalidEnumArgumentException(nameof(template), (int)template, typeof(DotNetNewTemplate)),
        };
    }

    private async Task<string> NewAspNetCoreWebApiNativeAotAsync(DotNetCliOptions options)
    {
        options.ThrowIfNoName();

        string project = options.Name;

        using Process process = StartProcess("dotnet", ["new", "webapiaot", "--output", _directory.FullName, "--language", "C#", "--framework", "net8.0", "--exclude-launch-settings"], options);

        TimeSpan timeout = OperatingSystem.IsWindows() && TestEnvironment.IsContinuousIntegration
            ? TimeSpan.FromSeconds(30)
            : TimeSpan.FromSeconds(10);
        await process.WaitForSuccessfulExitAsync(timeout);

        return project;
    }

    private async Task<string> NewSolutionAsync(DotNetCliOptions options)
    {
        options.ThrowIfNoName();

        string solution = options.Name;

        string templateShortName = Random.Shared.NextBoolean() ? "sln" : "solution";
        using Process process = StartProcess("dotnet", ["new", templateShortName], options);

        TimeSpan timeout = OperatingSystem.IsWindows() && TestEnvironment.IsContinuousIntegration
            ? TimeSpan.FromSeconds(10)
            : TimeSpan.FromSeconds(05);
        await process.WaitForSuccessfulExitAsync(timeout);

        return solution;
    }
}
