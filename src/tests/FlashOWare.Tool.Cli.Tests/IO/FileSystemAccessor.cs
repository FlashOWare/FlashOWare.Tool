using FlashOWare.Tool.Cli.IO;

namespace FlashOWare.Tool.Cli.Tests.IO;

internal sealed class FileSystemAccessor : IFileSystemAccessor
{
    private readonly DirectoryInfo _directory;

    public FileSystemAccessor(DirectoryInfo directory)
    {
        _directory = directory;
    }

    public DirectoryInfo GetCurrentDirectory()
    {
        return _directory;
    }
}
