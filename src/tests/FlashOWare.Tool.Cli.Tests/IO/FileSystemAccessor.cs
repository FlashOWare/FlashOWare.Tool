using FlashOWare.Tool.Cli.IO;

namespace FlashOWare.Tool.Cli.Tests.IO;

internal sealed class FileSystemAccessor(DirectoryInfo directory) : IFileSystemAccessor
{
    private readonly DirectoryInfo _directory = directory;

    public DirectoryInfo GetCurrentDirectory()
    {
        return _directory;
    }
}
