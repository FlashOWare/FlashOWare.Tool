namespace FlashOWare.Tool.Cli.IO;

internal static class FileSystemAccessorExtensions
{
    public static FileInfo GetSingleProject(this IFileSystemAccessor fileSystem)
    {
        var currentDirectory = fileSystem.GetCurrentDirectory();
        var files = currentDirectory.GetFiles("*.*proj");

        FileInfo project = files switch
        {
            [] => throw new InvalidOperationException("Specify a project file. The current working directory does not contain a project file."),
            [var file] => file,
            [..] => throw new InvalidOperationException("Specify which project file to use because this folder contains more than one project file."),
        };

        return project;
    }
}
