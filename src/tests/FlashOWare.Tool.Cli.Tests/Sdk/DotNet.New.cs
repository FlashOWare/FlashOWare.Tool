using NuGet.Packaging;
using System.ComponentModel;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Sdk;

public partial class DotNet
{
    internal Task<string> NewAsync(DotNetNewTemplate template)
    {
        return template switch
        {
            DotNetNewTemplate.Unspecified => throw new InvalidOperationException("Template not specified."),
            DotNetNewTemplate.AspNetCoreWebApiNativeAot => NewAspNetCoreWebApiNativeAotAsync(),
            _ => throw new InvalidEnumArgumentException(nameof(template), (int)template, typeof(DotNetNewTemplate)),
        };
    }

    internal async Task<string> NewAspNetCoreWebApiNativeAotAsync()
    {
        string project = "WebApiAotProject";

        ProcessStartInfo startInfo = new("dotnet");
        startInfo.ArgumentList.AddRange(new[] { "new", "webapiaot", "--name", project, "--output", _directory.FullName, "--language", "C#", "--framework", "net8.0", "--exclude-launch-settings" });
        startInfo.WorkingDirectory = _directory.FullName;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;

        using var process = Process.Start(startInfo);
        if (process is null)
        {
            throw new InvalidOperationException("No process resource is started.");
        }

        using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(15));

        await process.WaitForExitAsync(timeout.Token);

        if (process.ExitCode != 0)
        {
            string output = await process.StandardOutput.ReadToEndAsync(timeout.Token);
            throw new InvalidOperationException($"{nameof(Process.ExitCode)}: {process.ExitCode}{Environment.NewLine}{output}");
        }

        return project;
    }
}
