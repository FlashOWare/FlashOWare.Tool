using FlashOWare.Tool.Cli.Tests.IO;
using FlashOWare.Tool.Cli.Tests.Testing;
using FlashOWare.Tool.Cli.Tests.Text;
using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.Workspaces;

internal sealed class PhysicalProject
{
    public static PhysicalProject Create(DirectoryInfo directory, string name, Language language)
    {
        string extension = language.GetProjectExtension(true);
        string fileName = PathUtilities.WithExtension(extension, name);

        string path = Path.Combine(directory.FullName, fileName);
        return new PhysicalProject(path);
    }

    private PhysicalProject(string filePath)
        : this(new FileInfo(filePath))
    {
    }

    private PhysicalProject(FileInfo file)
    {
        File = file;
    }

    public FileInfo File { get; }
    public string Name => Path.GetFileNameWithoutExtension(File.Name);

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

        System.IO.File.WriteAllText(File.FullName, text, Encodings.UTF8NoBOM);
    }
}
