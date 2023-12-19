using FlashOWare.Tool.Cli.Tests.IO;
using FlashOWare.Tool.Cli.Tests.MSBuild;
using FlashOWare.Tool.Cli.Tests.Sdk;
using FlashOWare.Tool.Cli.Tests.Workspaces;
using System.CommandLine.IO;
using Xunit.Abstractions;

namespace FlashOWare.Tool.Cli.Tests.Testing;

public abstract class IntegrationTests : IDisposable
{
    private static int s_number = 0;

    private readonly DirectoryInfo _scratch;
    private readonly FileSystemAccessor _fileSystem;
    private readonly RedirectedConsole _system;

    protected IntegrationTests()
        : this(null)
    {
    }

    protected IntegrationTests(ITestOutputHelper? output)
    {
        MSBuild = MSBuildInfo.Create();
        Output = output;
        Console = new TestConsole();

        int incremented = Interlocked.Increment(ref s_number);

        Type type = GetType();
        string name = type.FullName ?? type.Name;

        _scratch = FileSystemUtilities.CreateScratchDirectory(Build.Configuration, Build.TFM, name, incremented);
        _fileSystem = new FileSystemAccessor(_scratch);
        Workspace = new PhysicalWorkspaceProvider(_scratch);
        DotNet = new DotNet(_scratch);

        Result = new RunResult();

        _system = new RedirectedConsole();
    }

    protected MSBuildInfo MSBuild { get; }

    protected ITestOutputHelper? Output { get; }

    protected TestConsole Console { get; }

    protected PhysicalWorkspaceProvider Workspace { get; }

    protected DotNet DotNet { get; }

    protected RunResult Result { get; }

    protected async Task RunAsync(params string[] args)
    {
        int exitCode = await CliApplication.RunAsync(args, Console, MSBuild.Instance, _fileSystem);

        Result.Set(exitCode);
    }

    void IDisposable.Dispose()
    {
        if (!_scratch.EnumerateFileSystemInfos().Any())
        {
            _scratch.Delete(false);
        }

        _system.AssertEmpty();
        _system.Dispose();

        Output?.WriteLine("MSBuild ({0}): {1} {2}", MSBuild.Instance.DiscoveryType, MSBuild.Instance.Name, MSBuild.Instance.Version);
    }
}
