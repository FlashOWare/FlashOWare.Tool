using FlashOWare.Tool.Cli.Tests.Extensions;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Sdk;

public sealed partial class DotNet
{
    private static readonly SemaphoreSlim s_synchronizationMutex = new(1, 1);

    private readonly DirectoryInfo _directory;

    public DotNet(DirectoryInfo directory)
    {
        _directory = directory;
    }

    private Process StartProcess(string fileName, params string[] arguments)
    {
        return StartProcess(fileName, arguments, DotNetCliOptions.None);
    }

    private Process StartProcess(string fileName, string[] arguments, DotNetCliOptions options)
    {
#if NET8_0_OR_GREATER
        ProcessStartInfo startInfo = new(fileName, arguments)
#else
        ProcessStartInfo startInfo = new(fileName)
#endif
        {
            WorkingDirectory = _directory.FullName,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
        };

#if !NET8_0_OR_GREATER
        startInfo.ArgumentList.AddRange(arguments);
#endif
        options.AddTo(startInfo.ArgumentList);

        var process = Process.Start(startInfo);

        if (process is null)
        {
            throw new InvalidOperationException("No process resource is started.");
        }

        return process;
    }
}
