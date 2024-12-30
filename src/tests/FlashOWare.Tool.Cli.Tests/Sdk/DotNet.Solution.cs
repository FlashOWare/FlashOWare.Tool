using FlashOWare.Tool.Cli.Tests.Diagnostics;
using FlashOWare.Tool.Cli.Tests.Hosting;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Sdk;

public partial class DotNet
{
    internal async Task SlnAddAsync(FileInfo solution, string project)
    {
        using Process process = StartProcess("dotnet", "sln", solution.FullName, "add", project);

        TimeSpan timeout = OperatingSystem.IsWindows() && TestEnvironment.IsContinuousIntegration
            ? TimeSpan.FromSeconds(10)
            : TimeSpan.FromSeconds(05);
        await process.WaitForSuccessfulExitAsync(timeout);
    }

    internal async Task SlnRemoveAsync(FileInfo solution, string project)
    {
        using Process process = StartProcess("dotnet", "sln", solution.FullName, "remove", project);

        TimeSpan timeout = OperatingSystem.IsWindows() && TestEnvironment.IsContinuousIntegration
            ? TimeSpan.FromSeconds(10)
            : TimeSpan.FromSeconds(05);
        await process.WaitForSuccessfulExitAsync(timeout);
    }
}
