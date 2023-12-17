using NuGet.Packaging;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Sdk;

public sealed partial class DotNet
{
    private readonly DirectoryInfo _directory;

    public DotNet(DirectoryInfo directory)
    {
        _directory = directory;
    }

    private Process StartProcess(string fileName, params string[] arguments)
    {
        ProcessStartInfo startInfo = new(fileName)
        {
            WorkingDirectory = _directory.FullName,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
        };

        if (arguments.Length != 0)
        {
            startInfo.ArgumentList.AddRange(arguments);
        }

        var process = Process.Start(startInfo);

        if (process is null)
        {
            throw new InvalidOperationException("No process resource is started.");
        }

        return process;
    }
}
