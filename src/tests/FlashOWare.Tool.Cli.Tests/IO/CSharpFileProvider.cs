using FlashOWare.Tool.Cli.Tests.Testing;

namespace FlashOWare.Tool.Cli.Tests.IO;

internal sealed class CSharpFileProvider
{
    private readonly DirectoryInfo _directory;

    private int _count;

    public CSharpFileProvider(DirectoryInfo directory)
    {
        _directory = directory;
    }

    public FileInfo CreateProject(string text, string name)
    {
        string fileName = Path.HasExtension(name) ? name : Path.ChangeExtension(name, Names.CSharpProjectExtension);
        string path = Path.Combine(_directory.FullName, fileName);

        File.WriteAllText(path, text);

        return new FileInfo(path);
    }

    public void CreateDocument(string text, string? name = null, params string[] folders)
    {
        string fileName = name is not null
            ? Path.HasExtension(name) ? name : Path.ChangeExtension(name, Names.CSharpFileExtension)
            : CreateFileName();

        if (folders.Length == 0)
        {
            string path = Path.Combine(_directory.FullName, fileName);
            File.WriteAllText(path, text);
        }
        else
        {
            string directory = folders.Aggregate(_directory.FullName, static (aggregate, element) => Path.Combine(aggregate, element));
            string path = Path.Combine(directory, fileName);

            Directory.CreateDirectory(directory);
            File.WriteAllText(path, text);
        }
    }

    private string CreateFileName()
    {
        int incremented = Interlocked.Increment(ref _count);
        return $"Test{incremented}.{Names.CSharpFileExtension}";
    }
}
