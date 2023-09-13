using FlashOWare.Tool.Cli.Tests.IO;
using FlashOWare.Tool.Cli.Tests.Testing;

namespace FlashOWare.Tool.Cli.Tests.Workspaces;

internal sealed class PhysicalProject
{
    public PhysicalProject(string filePath)
    {
        File = new FileInfo(filePath);
    }

    public FileInfo File { get; }

    public static PhysicalProject Create(DirectoryInfo directory, string name, Language language)
    {
        string extension = language.GetProjectExtension(true);
        string fileName = PathUtilities.WithExtension(extension, name);

        string path = Path.Combine(directory.FullName, fileName);
        return new PhysicalProject(path);
    }
}
