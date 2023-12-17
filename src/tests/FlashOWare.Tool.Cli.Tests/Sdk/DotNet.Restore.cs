using FlashOWare.Tool.Cli.Tests.Diagnostics;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Sdk;

public partial class DotNet
{
    internal async Task RestoreAsync(FileInfo project)
    {
        using Process process = StartProcess("dotnet", "restore", project.FullName);

        await process.WaitForSuccessfulExitAsync(TimeSpan.FromSeconds(15));
    }
}
