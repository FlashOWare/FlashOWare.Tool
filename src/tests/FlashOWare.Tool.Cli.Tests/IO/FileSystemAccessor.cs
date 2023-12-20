using FlashOWare.Tool.Cli.IO;

namespace FlashOWare.Tool.Cli.Tests.IO;

internal sealed class FileSystemAccessor(DirectoryInfo directory) : IFileSystemAccessor
{
    public DirectoryInfo GetCurrentDirectory()
    {
        return directory;
    }
}
