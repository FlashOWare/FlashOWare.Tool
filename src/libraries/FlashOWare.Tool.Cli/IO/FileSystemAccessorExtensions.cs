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

    public static FileInfo GetSingleSolution(this IFileSystemAccessor fileSystem)
    {
        var currentDirectory = fileSystem.GetCurrentDirectory();
        var files = currentDirectory.GetFiles("*.sln");

        FileInfo solution = files switch
        {
            [] => throw new InvalidOperationException("Specify a solution file. The current working directory does not contain a solution file."),
            [var file] => file,
            [..] => throw new InvalidOperationException("Specify which solution file to use because this folder contains more than one solution file."),
        };

        return solution;
    }

    public static FileInfo GetSingleSolutionFilter(this IFileSystemAccessor fileSystem)
    {
        var currentDirectory = fileSystem.GetCurrentDirectory();
        var files = currentDirectory.GetFiles("*.slnf");

        FileInfo solutionFilter = files switch
        {
            [] => throw new InvalidOperationException("Specify a solution filter file. The current working directory does not contain a solution filter file."),
            [var file] => file,
            [..] => throw new InvalidOperationException("Specify which solution filter file to use because this folder contains more than one solution filter file."),
        };

        return solutionFilter;
    }

    public static FileInfo[] GetProjects(this IFileSystemAccessor fileSystem)
    {
        var currentDirectory = fileSystem.GetCurrentDirectory();
        var files = currentDirectory.GetFiles("*.*proj");
        return files;
    }

    public static FileInfo[] GetSolutions(this IFileSystemAccessor fileSystem)
    {
        var currentDirectory = fileSystem.GetCurrentDirectory();
        var files = currentDirectory.GetFiles("*.sln");
        return files;
    }

    public static FileInfo[] GetSolutionFilters(this IFileSystemAccessor fileSystem)
    {
        var currentDirectory = fileSystem.GetCurrentDirectory();
        var files = currentDirectory.GetFiles("*.slnf");
        return files;
    }
}
