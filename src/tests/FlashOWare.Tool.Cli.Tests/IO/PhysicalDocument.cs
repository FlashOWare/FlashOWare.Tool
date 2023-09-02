using System.Diagnostics;

namespace FlashOWare.Tool.Cli.Tests.IO;

internal sealed class PhysicalDocument
{
    public PhysicalDocument(string text, string filePath)
    {
        Text = text;
        FilePath = filePath;
    }

    public PhysicalDocument(string text, string name, params string[] folders)
    {
        Debug.Assert(folders.Length != 0, $"Length of {nameof(folders)} is 0.");

        string directory = folders.Aggregate(static (aggregate, element) => Path.Combine(aggregate, element));

        Text = text;
        FilePath = Path.Combine(directory, name);
    }

    public PhysicalDocument(FileInfo file)
    {
        Text = File.ReadAllText(file.FullName);
        FilePath = file.FullName;
    }

    public string Text { get; }
    public string FilePath { get; }
}
