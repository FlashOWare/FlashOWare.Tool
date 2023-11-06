namespace FlashOWare.Tool.Cli.IO;

internal sealed class FileSystemAccessor : IFileSystemAccessor
{
    public static FileSystemAccessor System { get; } = new FileSystemAccessor();

    private FileSystemAccessor()
    {
    }

    public DirectoryInfo GetCurrentDirectory()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        return new DirectoryInfo(currentDirectory);
    }
}
