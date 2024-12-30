using FlashOWare.Tool.Cli.Tests.IO;

namespace FlashOWare.Tool.Cli.Tests.Workspaces;

internal sealed class PhysicalSolution
{
    public static PhysicalSolution Create(DirectoryInfo directory, string name)
    {
        const string extension = ".sln";
        string fileName = PathUtilities.WithExtension(extension, name);

        string path = Path.Combine(directory.FullName, fileName);
        return new PhysicalSolution(path);
    }

    private PhysicalSolution(string filePath)
        : this(new FileInfo(filePath))
    {
    }

    private PhysicalSolution(FileInfo file)
    {
        File = file;
    }

    public FileInfo File { get; }
    public string FullName => File.FullName;
    public string Name => Path.GetFileNameWithoutExtension(File.Name);
}
