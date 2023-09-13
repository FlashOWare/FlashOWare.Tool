using FlashOWare.Tool.Cli.Tests.IO;
using FlashOWare.Tool.Cli.Tests.Testing;

namespace FlashOWare.Tool.Cli.Tests.Workspaces;

public sealed class PhysicalWorkspaceProvider
{
    private readonly DirectoryInfo _directory;

    public PhysicalWorkspaceProvider(DirectoryInfo directory)
    {
        _directory = directory;
    }

    internal PhysicalProjectBuilder CreateProject(Language language = Language.CSharp)
    {
        return new PhysicalProjectBuilder(_directory, language);
    }

    internal PhysicalSolutionBuilder CreateSolution()
    {
        return new PhysicalSolutionBuilder(_directory);
    }

    internal FileSystemExpectation CreateExpectation(Language language = Language.CSharp)
    {
        return new FileSystemExpectation(_directory, language);
    }
}
