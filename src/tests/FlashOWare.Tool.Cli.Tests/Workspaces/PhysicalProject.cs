using FlashOWare.Tool.Cli.Tests.IO;
using FlashOWare.Tool.Cli.Tests.Testing;
using System.Diagnostics;

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

    public string GetDirectoryName()
    {
        string? directory = File.DirectoryName;
        Debug.Assert(directory is not null, $"'{File}' is in a root directory.");
        return directory;
    }

    public void Write(string text)
    {
        if (File.Exists)
        {
            throw new InvalidOperationException($"Project '{File}' already exists.");
        }

        System.IO.File.WriteAllText(File.FullName, text);
    }
}
