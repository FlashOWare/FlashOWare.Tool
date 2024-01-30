using FlashOWare.Tool.Cli.Tests.Diagnostics;
using FlashOWare.Tool.Cli.Tests.Hosting;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Sdk;

public partial class DotNet
{
    internal async Task RestoreAsync(FileInfo project)
    {
        using Process process = StartProcess("dotnet", "restore", project.FullName);

        TimeSpan timeout = OperatingSystem.IsWindows() && TestEnvironment.IsContinuousIntegration
            ? TimeSpan.FromSeconds(15)
            : TimeSpan.FromSeconds(05);
        await process.WaitForSuccessfulExitAsync(timeout);
    }
}
