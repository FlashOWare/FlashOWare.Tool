using FlashOWare.Tool.Cli.Tests.IO;
using Microsoft.CodeAnalysis;
using System.CommandLine.IO;
using Xunit.Abstractions;

namespace FlashOWare.Tool.Cli.Tests.Testing;

public abstract class IntegrationTests : IDisposable
{
    private static int s_number = 0;

    private readonly DirectoryInfo _scratch;
    private readonly RedirectedConsole _system;

    protected IntegrationTests()
        : this(null)
    {
    }

    protected IntegrationTests(ITestOutputHelper? output)
    {
        Output = output;
        Console = new TestConsole();

        int incremented = Interlocked.Increment(ref s_number);

        Type type = GetType();
        string name = type.FullName ?? type.Name;

        _scratch = FileSystemUtilities.CreateScratchDirectory(Build.Configuration, Build.TFM, name, incremented);
        Workspace = new FileSystemWorkspace(_scratch);

        _system = new RedirectedConsole();
    }

    protected ITestOutputHelper? Output { get; }

    protected TestConsole Console { get; }

    protected FileSystemWorkspace Workspace { get; }

    void IDisposable.Dispose()
    {
        if (!_scratch.EnumerateFileSystemInfos().Any())
        {
            _scratch.Delete(false);
        }

        _system.AssertEmpty();
        _system.Dispose();
    }

    public string GetScratchPath(string path)
    {
        return Path.Combine(_scratch.FullName, path);
    }
}
