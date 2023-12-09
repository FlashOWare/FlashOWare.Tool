using NuGet.Packaging;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Sdk;

public partial class DotNet
{
    internal async Task RestoreAsync(FileInfo project)
    {
        ProcessStartInfo startInfo = new("dotnet");
        startInfo.ArgumentList.AddRange(new[] { "restore", project.FullName });
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
    }
}
