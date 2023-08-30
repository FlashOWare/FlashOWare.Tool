using FlashOWare.Tool.Cli.Tests.Assertions;
using Microsoft.CodeAnalysis;

namespace FlashOWare.Tool.Cli.Tests.IO;

public sealed class FileSystemWorkspace
{
    private readonly DirectoryInfo _directory;

    public FileSystemWorkspace(DirectoryInfo directory)
    {
        _directory = directory;

        CSharp = new CSharpFileProvider(directory);
        VisualBasic = new VisualBasicFileProvider(directory);
    }

    internal CSharpFileProvider CSharp { get; }

    internal VisualBasicFileProvider VisualBasic { get; }

    internal void AssertFiles(PhysicalDocumentList expected)
    {
        PhysicalDocument[] actual = _directory.EnumerateFiles("*", SearchOption.AllDirectories)
            .Where((FileInfo file) =>
            {
                string path = Path.GetRelativePath(_directory.FullName, file.FullName);
                return !path.StartsWith($"bin{Path.DirectorySeparatorChar}") && !path.StartsWith($"obj{Path.DirectorySeparatorChar}");
            })
            .Select(static (FileInfo file) => new PhysicalDocument(file))
            .ToArray();

        Diff.Assert(expected.Documents, actual, _directory);
    }
}
