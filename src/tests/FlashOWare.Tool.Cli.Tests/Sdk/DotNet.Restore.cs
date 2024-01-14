using FlashOWare.Tool.Cli.Tests.Diagnostics;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Sdk;

public partial class DotNet
{
    private static readonly SemaphoreSlim s_restoreMutex = new(1, 1);

    internal async Task RestoreAsync(FileInfo project)
    {
        using Process process = StartProcess("dotnet", "restore", project.FullName);

        await process.WaitForSuccessfulExitAsync(TimeSpan.FromSeconds(15));
    }

    internal async Task SynchronizedRestoreAsync(FileInfo project)
    {
        await s_restoreMutex.WaitAsync();

        try
        {
            await RestoreAsync(project);
        }
        finally
        {
            _ = s_restoreMutex.Release();
        }
    }
}
